export interface PredictionResponse {
    Request: PredictionRequest;
    Predictions: PredictionResponseItem[];
}

export interface PredictionResponseItem {
    ProbableSuccess: boolean;
    SuccessProbability: number;
}

export interface PredictionRequest {
    ProjectName: string;
    Items: PredictionRequestFile[];
}

export interface PredictionRequestFile {
    Author: string;
    Path: string;
    OldPath: string;
    NumberOfModifiedLines: number;
    CCMMax?: number;
    CCMMd?: number;
    CCMAvg?: number;
}

export interface TelemetryRequest {
    UserName: string;
    UserMachineName: string;
    Action: string;
    Payload?: string;
    VisualStudioVersion: string;
    PluginVersion: string;
}