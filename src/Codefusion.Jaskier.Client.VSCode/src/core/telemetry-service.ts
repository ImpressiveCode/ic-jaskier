import { HttpService } from './http-service';
import { TelemetryRequest } from '../api';
import { EventEmitter } from 'vscode';
import * as vscode from 'vscode';
import * as system from 'os';

export class TelemetryService {
    public errors = new EventEmitter<any>();
    private telemetryData: TelemetryRequest;

    constructor(private httpService: HttpService) {
        let extensionVersion = vscode.extensions.getExtension('CODEFUSION.jaskier').packageJSON.version;

        this.telemetryData = {
            UserName: system.userInfo().username,
            UserMachineName: system.hostname(),
            Action: null,
            VisualStudioVersion: vscode.version,
            PluginVersion: extensionVersion
        }
    }

    public sendData(actionName: string) {
        let request = this.createRequest(actionName);
        this.httpService.sendTelemetryRequest(request,
            () => this.handleResponse(request),
            error => this.handleError(error));
    }

    private createRequest(actionName: string): TelemetryRequest {
        this.telemetryData.Action = actionName;

        return this.telemetryData;
    }

    private handleResponse(request: TelemetryRequest) {
        console.log('Successfully sent telemetry data:' + JSON.stringify(request));
    }

    private handleError(error) {
        this.errors.fire(error);
    }
}