import { PredictionResponse, PredictionResponseItem, PredictionRequestFile } from './api';

export class Predictions {
    public items: PredictionItemData[] = [];

    // Changed file that raised the predictions fetching.
    public changedFile: string;

    constructor(response: PredictionResponse, changedFile: string) {
        this.changedFile = changedFile;

        if (response == null) {
            return;
        }

        for (let loopIndex = 0; loopIndex < response.Predictions.length; loopIndex++) {
            let prediction = response.Predictions[loopIndex];
            let file = response.Request.Items[loopIndex];

            if (!prediction || !file || !file.Path) {
                continue;
            }

            let itemData = new PredictionItemData(prediction, file);
            this.items.push(itemData);
        }
    }

    public isEmpty(): boolean {
        return this.items == null || this.items.length === 0;
    }
}

export class PredictionItemData {
    public probableSuccess: boolean;
    public successProbability: number;

    public numberOfModifiedLines: number;
    public path: string;

    public maxCyclomaticComplexity?: number;
    public averageCyclomaticComplexity?: number;
    public medianCyclomaticComplexity?: number;

    constructor(prediction: PredictionResponseItem, fileData: PredictionRequestFile) {
        this.probableSuccess = prediction.ProbableSuccess;
        this.successProbability = prediction.SuccessProbability;

        this.numberOfModifiedLines = fileData.NumberOfModifiedLines;
        this.path = fileData.Path;

        this.maxCyclomaticComplexity = fileData.CCMMax;
        this.averageCyclomaticComplexity = fileData.CCMAvg;
        this.medianCyclomaticComplexity = fileData.CCMMd;
    }
}