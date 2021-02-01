import { PredictionsPresenter } from '../results/predictions-presenter';
import { ChangesTracker, ChangedFiles, ChangedFileInfo } from '../changed-files.model';
import * as vscode from 'vscode';
import { FileWatcher } from './file-watcher';
import { GitChangesTracker } from './git-changes-tracker';
import { OutputChannelService } from '../results/output-channel-service';
import { Disposable } from 'vscode';
import { PredictionsService } from './predictions-service';
import { HttpService } from './http-service';
import { CodeMetricsProvider } from './code-metrics-provider';
import { Predictions } from '../predictions.model';
import { TelemetryService } from './telemetry-service';
import { Logger } from './logger';

export class ExtensionManager implements Disposable {
    private fileWatcher: FileWatcher;
    private changesTracker: ChangesTracker;
    private predictionsPresenter: PredictionsPresenter;
    private outputChannelService: OutputChannelService;
    private subscriptions: Disposable[];
    private predictionsService: PredictionsService;
    private codeMetricsProvider: CodeMetricsProvider;
    private telemetryService: TelemetryService;

    private lastChanges: ChangedFiles = null;

    public setup() {
        let repositoryPath = vscode.workspace.workspaceFolders[0].uri.fsPath;
        this.fileWatcher = new FileWatcher();
        this.fileWatcher.setup(fileUri => this.handleFileChange(fileUri));

        this.changesTracker = new GitChangesTracker(repositoryPath);
        let httpService = new HttpService();
        this.predictionsService = new PredictionsService(httpService);
        this.telemetryService = new TelemetryService(httpService);
        this.outputChannelService = new OutputChannelService(this.telemetryService);
        this.codeMetricsProvider = new CodeMetricsProvider();
        this.predictionsPresenter = new PredictionsPresenter(this.outputChannelService, this.predictionsService);
        let predictionsSubsription = this.predictionsService.predictions.event(result => this.handleNewPredictions(result));
        let predictionsErrorsSubsription = this.predictionsService.errors.event(error => this.handleError(error));
        let telemetryErrorsSubsription = this.telemetryService.errors.event(error => this.handleError(error));

        let showOutputCommand = vscode.commands.registerCommand('jaskier.showOutputChannel', () => {
            this.outputChannelService.showOutputChannel();
        });

        let refreshPredictionsCommand = vscode.commands.registerCommand('jaskier.refreshPredictions', () => {
            this.loadPredictions(null, true);
        });

        this.subscriptions = [showOutputCommand, refreshPredictionsCommand, predictionsSubsription,
            predictionsErrorsSubsription, telemetryErrorsSubsription];
    }

    public loadPredictions(changedFile: string = null, force = false) {
        this.changesTracker.getChangedFiles()
            .then(data => this.handleChangedFiles(data, changedFile, force))
            .catch(error => {
                this.handleError(error);
            });
    }

    public dispose() {
        let disposables: Disposable[] = [
            this.predictionsPresenter,
            this.outputChannelService,
            ...this.subscriptions,
            this.fileWatcher
        ];

        disposables.forEach(loopItem => {
            if (loopItem) {
                loopItem.dispose();
            }
        });
    }

    private handleFileChange(fileUri: vscode.Uri) {
        this.loadPredictions(fileUri.fsPath);
    }

    private handleNewPredictions(predictions: Predictions) {
        Logger.logPredictions(predictions);
        this.predictionsPresenter.publishResults(predictions);
    }

    private handleError(error: any) {
        console.log(error);
        this.outputChannelService.appendMessages('Error occurred.', error);
        this.predictionsPresenter.resetStatusBarItem();
    }

    private handleChangedFiles(changedFiles: ChangedFiles, changedFile: string, force: boolean) {
        // Do not send request if changes are equal to previous ones.
        if (!force && changedFiles.equals(this.lastChanges)) {
            Logger.logChangesEqualNotification(changedFile, this.outputChannelService);
            return;
        }

        this.codeMetricsProvider.loadCodeMetrics(changedFiles);
        this.lastChanges = changedFiles;
        Logger.logChanges(changedFiles.files);

        let projectName = vscode.workspace.name;
        this.predictionsService.requestPredictions(projectName, changedFiles, changedFile);
    }
}