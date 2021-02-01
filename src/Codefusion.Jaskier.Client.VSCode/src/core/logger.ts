import { ChangedFileInfo } from '../changed-files.model';
import { Predictions } from '../predictions.model';
import * as vscode from 'vscode';
import { OutputChannelService } from '../results/output-channel-service';

export class Logger {
    public static createEventSourceMessage(changedFile: string) {
        if (changedFile != null) {
            return `Raised by change event of: "${changedFile}"`;
        }

        return `Raised on demand`;
    }

    public static logChanges(changes: ChangedFileInfo[]) {
        console.log('ChangedFiles:');
        changes.forEach(loopFile => {
            console.log(`\t${JSON.stringify(loopFile)}`);
        });
    }

    public static logPredictions(result: Predictions) {
        if (!result || result.isEmpty()) {
            console.log('No predictions received.');
            return;
        }

        console.log('Predictions:');
        result.items.forEach(loopPrediction => {
            console.log(`\t${JSON.stringify(loopPrediction)}`);
        });
    }

    public static logChangesEqualNotification(changedFile: string, outputChannelService: OutputChannelService) {
        let eventSourceMessage = Logger.createEventSourceMessage(changedFile);
        let message = 'Changes are equal to the previous ones - request will not be sent.';
        console.log(eventSourceMessage, message);

        if (Logger.isDebugModeEnabled()) {
            outputChannelService.appendMessages(eventSourceMessage, message);
        }
    }

    private static isDebugModeEnabled(): boolean {
        return vscode.workspace.getConfiguration('jaskier').debugMode;
    }
}