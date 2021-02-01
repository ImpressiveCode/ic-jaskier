import { PredictionRequest, PredictionRequestFile, PredictionResponse } from '../api';
import { request as httpRequest, IncomingMessage, RequestOptions } from 'http';
import { HttpService } from './http-service';
import { EventEmitter } from 'vscode';
import { Predictions } from '../predictions.model';
import { ChangedFiles } from '../changed-files.model';

interface RequestProperties {
    projectName: string;
    changedFiles: ChangedFiles;
    // Changed file that raised the predictions fetching.
    changedFile: string;
}

export class PredictionsService {
    public predictions = new EventEmitter<Predictions>();
    public errors = new EventEmitter<any>();
    public isPullingPredictionsChanged = new EventEmitter<boolean>();

    private isPullingPredictions = false;
    private pendingRequest: RequestProperties = null;

    constructor(private httpService: HttpService) {
        this.predictions.event(result => this.handleRequestCompleted());
        this.errors.event(error => this.handleRequestCompleted());
    }

    public requestPredictions(projectName: string, changedFiles: ChangedFiles, changedFile: string) {
        if (this.isPullingPredictions) {
            this.pendingRequest = { projectName, changedFiles, changedFile };
            return;
        }

        this.isPullingPredictions = true;
        this.isPullingPredictionsChanged.fire(true);
        this.pendingRequest = null;

        if (!changedFiles || !changedFiles.files || changedFiles.files.length === 0) {
            this.predictions.fire(this.createEmptyResult(changedFile));
            return;
        }

        let request = this.buildRequest(projectName, changedFiles);
        this.sendRequest(request, changedFile);
    }

    private buildRequest(projectName: string, changedFiles: ChangedFiles): PredictionRequest {
        return {
            ProjectName: projectName,
            Items: this.mapFiles(changedFiles)
        };
    }

    private mapFiles(changedFiles: ChangedFiles): PredictionRequestFile[] {
        return changedFiles.files.map(loopFile => {
            return <PredictionRequestFile>{
                Author: loopFile.author,
                Path: loopFile.path,
                OldPath: loopFile.oldPath,
                NumberOfModifiedLines: loopFile.numberOfModifiedLines,
                CCMMax: loopFile.maxCyclomaticComplexity,
                CCMAvg: loopFile.averageCyclomaticComplexity,
                CCMMd: loopFile.medianCyclomaticComplexity
            };
        });
    }

    private sendRequest(requestData: PredictionRequest, changedFile: string) {
        this.httpService.sendPredictionRequest(requestData,
            response => {
                if (response == null) {
                    this.predictions.fire(this.createEmptyResult(changedFile));
                    return;
                }

                this.predictions.fire(new Predictions(response, changedFile));
            },
            error => {
                this.errors.fire(error);
            }
        );
    }

    private createEmptyResult(changedFile: string): Predictions {
        return new Predictions(null, changedFile);
    }

    private handleRequestCompleted() {
        this.isPullingPredictions = false;
        this.isPullingPredictionsChanged.fire(false);

        if (this.pendingRequest != null) {
            this.requestPredictions(this.pendingRequest.projectName,
                this.pendingRequest.changedFiles,
                this.pendingRequest.changedFile);
        }
    }
}