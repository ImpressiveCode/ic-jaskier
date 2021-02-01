import { PredictionResponseItem } from '../api';
import * as vscode from 'vscode';
import { StatusBarAlignment, Disposable, StatusBarItem, OutputChannel, Color } from 'vscode';
import { OutputChannelService } from './output-channel-service';
import { Predictions, PredictionItemData } from '../predictions.model';
import { OutputResultsBuilder } from './output-results-builder';
import { OutputResultsWithDebugDataBuilder } from './output-results-with-debug-data-builder';
import { PredictionsService } from '../core/predictions-service';

export class PredictionsPresenter implements Disposable {
    private statusBarItem: StatusBarItem;
    private outputResultsBuilder = new OutputResultsBuilder();
    private outputResultsWithDebugDataBuilder = new OutputResultsWithDebugDataBuilder();

    constructor(private outputChannelService: OutputChannelService, private predictionsService: PredictionsService) {
        this.statusBarItem = vscode.window.createStatusBarItem(StatusBarAlignment.Left, 0);
        this.statusBarItem.command = 'jaskier.showOutputChannel';
        this.setDefaultTextForStatusBarItem();
        this.statusBarItem.show();
        this.predictionsService.isPullingPredictionsChanged.event(newValue => this.handleDataLoadingChanged(newValue));
    }

    public publishResults(predictions: Predictions) {
        if (!predictions || predictions.isEmpty()) {

            let changedFile = predictions ? predictions.changedFile : null;
            this.handleNoPredictionsReceived(changedFile);
            return;
        }

        this.updateStatusBarItem(predictions.items);

        let resultsBuilder = this.getResultsBuilder();
        let results = resultsBuilder.buildResults(predictions);
        this.outputChannelService.appendMessages(...results);
    }

    public resetStatusBarItem() {
        this.setDefaultTextForStatusBarItem();
    }

    public dispose() {
        if (this.statusBarItem) {
            this.statusBarItem.dispose();
        }
    }

    private updateStatusBarItem(predictions: PredictionItemData[]) {
        let numberOfPredictions = predictions.filter(item => item.successProbability != null).length;
        if (numberOfPredictions === 0) {
            this.statusBarItem.text = '[Jaskier] No predictions';
            return;
        }

        let numberOfFails = predictions.filter(item => item.probableSuccess === false).length;
        let successProbabilities = predictions.map(item => item.successProbability);
        let minSuccessProbability = Math.min(...successProbabilities);
        let maxFailProbability = (1 - minSuccessProbability) * 100;
        let formattedMaxFailProbability = maxFailProbability.toFixed(0) + ' %';
        let text: string;

        if (numberOfFails === 0) {
            text = `[Jaskier] Low fail probability (${formattedMaxFailProbability})`;
        } else {
            text = `[Jaskier] $(alert) High fail probability (${formattedMaxFailProbability})`;
        }

        this.statusBarItem.text = text;
    }

    private handleNoPredictionsReceived(changedFile: string) {
        this.setDefaultTextForStatusBarItem();

        let resultsBuilder = this.getResultsBuilder();
        let message = resultsBuilder.buildNoDataMessage(changedFile);
        this.outputChannelService.appendMessages(...message);
    }

    private setDefaultTextForStatusBarItem() {
        this.statusBarItem.text = '[Jaskier]';
    }

    private handleDataLoadingChanged(newValue: boolean) {
        if (newValue) {
            this.statusBarItem.text = '[Jaskier] Waiting for predictions...';
        }
    }

    private getResultsBuilder(): OutputResultsBuilder {
        let debugModeEnabled = vscode.workspace.getConfiguration('jaskier').debugMode;

        if (debugModeEnabled) {
            return this.outputResultsWithDebugDataBuilder;
        }

        return this.outputResultsBuilder;
    }
}