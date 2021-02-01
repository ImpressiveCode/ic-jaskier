import { Predictions } from '../predictions.model';
import * as vscode from 'vscode';
import * as path from 'path';

export class OutputResultsBuilder {
    private repositoryPath = vscode.workspace.workspaceFolders[0].uri.fsPath;

    public buildResults(predictions: Predictions): string[] {
        let resultsHeader = 'Fail probability | File path';
        let results = [resultsHeader];
        let anyPrediction = false;

        predictions.items
            .filter(item => item.successProbability != null)
            .forEach(item => {
                let failProbability = (1 - item.successProbability) * 100;

                let prefix = !item.probableSuccess ? '[!]' : '   ';
                let failProbabilityFormatted = this.getWithPaddedSpaces(failProbability.toFixed(2));
                let fullPath = this.getFullPath(item.path);
                let message = `${prefix}    ${failProbabilityFormatted} % | "${fullPath}"`;
                results.push(message);
                anyPrediction = true;
            });

        if (!anyPrediction) {
            return ['- No predictions -'];
        }

        return results;
    }

    public buildNoDataMessage(changedFile: string): string[] {
        return ['- No changes or no predictions -'];
    }

    protected getWithPaddedSpaces(value: string): string {
        return ("   " + value).slice(-7);
    }

    protected getFullPath(relativePath: string): string {
        return path.join(this.repositoryPath, relativePath);
    }
}