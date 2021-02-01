import { Predictions, PredictionItemData } from '../predictions.model';
import { OutputResultsBuilder } from './output-results-builder';
import { Logger } from '../core/logger';

export class OutputResultsWithDebugDataBuilder extends OutputResultsBuilder {

    public buildResults(predictions: Predictions): string[] {
        let eventSourceMessage = Logger.createEventSourceMessage(predictions.changedFile);
        let resultsHeader = 'Fail probability | File path | Modified lines count | Cyclomatic Complexity { max, avg, median }';
        let results = [eventSourceMessage, resultsHeader];
        let anyPrediction = false;

        predictions.items
            .filter(item => item.successProbability != null)
            .forEach(item => {
                let failProbability = (1 - item.successProbability) * 100;

                let prefix = !item.probableSuccess ? '[!]' : '   ';
                let failProbabilityFormatted = this.getWithPaddedSpaces(failProbability.toFixed(2));
                let cyclomaticComplexityMessage = this.createCyclomaticComplexityMessage(item);

                let fullPath = this.getFullPath(item.path);
                let message = `${prefix}    ${failProbabilityFormatted} % | "${fullPath}" | ${item.numberOfModifiedLines} | ${cyclomaticComplexityMessage}`;
                results.push(message);
                anyPrediction = true;
            });

        if (!anyPrediction) {
            return [eventSourceMessage, '- No predictions -'];
        }

        return results;
    }

    public buildNoDataMessage(changedFile: string): string[] {
        let eventSourceMessage = Logger.createEventSourceMessage(changedFile);

        return [eventSourceMessage, '- No changes or no predictions -'];
    }

    private createCyclomaticComplexityMessage(item: PredictionItemData): string {
        if (item.maxCyclomaticComplexity == null &&
            item.averageCyclomaticComplexity == null &&
            item.medianCyclomaticComplexity == null) {

            return 'undefined';
        }

        let ccMax = item.maxCyclomaticComplexity;
        let ccAvg = item.averageCyclomaticComplexity;
        let ccMd = item.medianCyclomaticComplexity;

        return `{ ${ccMax}, ${ccAvg}, ${ccMd} }`;
    }
}