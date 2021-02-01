import { request as httpRequest, IncomingMessage, RequestOptions } from 'http';
import { PredictionRequest, PredictionResponse, TelemetryRequest } from '../api';
import { URL } from 'url';
import * as vscode from 'vscode';

export class HttpService {
    public sendPredictionRequest(requestData: PredictionRequest, resolve: (result?: PredictionResponse) => void, reject: (error: any) => void) {
        let rawRequestData = JSON.stringify(requestData);
        let requestOptions = this.prepareRequestOptions(rawRequestData, '/GetPredictions');
        let request = httpRequest(requestOptions,
            response => this.handlePredictionsResponse(response, resolve, reject, requestData));

        request.on('error', error => {
            reject('Http error: ' + error);
        });

        request.write(rawRequestData);
        request.end();
    }

    public sendTelemetryRequest(requestData: TelemetryRequest, resolve: () => void, reject: (error: any) => void) {
        let rawRequestData = JSON.stringify(requestData);
        let requestOptions = this.prepareRequestOptions(rawRequestData, '/PutTelemetry');
        let request = httpRequest(requestOptions,
            response => this.handleTelemetryResponse(response, resolve));

        request.on('error', error => {
            reject('Http error: ' + error);
        });

        request.write(rawRequestData);
        request.end();
    }

    private prepareRequestOptions(rawRequestData: string, actionPath: string): RequestOptions {
        let serviceUrl = new URL(vscode.workspace.getConfiguration('jaskier').serviceUrl);

        return <RequestOptions>{
            protocol: serviceUrl.protocol,
            hostname: serviceUrl.hostname,
            port: serviceUrl.port,
            method: 'POST',
            path: actionPath,
            headers: {
                'Content-Type': 'application/json',
                'Content-Length': Buffer.byteLength(rawRequestData)
            }
        }
    }

    private handlePredictionsResponse(response: IncomingMessage,
        resolve: (result?: PredictionResponse) => void,
        reject: (error: any) => void,
        request: PredictionRequest) {

        let data = '';

        // A chunk of data has been received.
        response.on('data', chunk => {
            data += chunk;
        });

        // The whole response has been received.
        response.on('end', () => {
            let result = this.extractResult(data, request, reject);

            if (result != null) {
                resolve(result);
            }
        });
    }

    private extractResult(responseData: string, request: PredictionRequest, reject: (error: any) => void): PredictionResponse {
        try {
            let parsedResponse = <PredictionResponse>JSON.parse(responseData);
            parsedResponse.Request = request;

            return parsedResponse;
        } catch (error) {
            reject('Invalid repsonse: ' + responseData);
            return null;
        }
    }

    private handleTelemetryResponse(response: IncomingMessage, resolve: () => void) {
        let data = '';

        // A chunk of data has been received.
        response.on('data', chunk => {
            data += chunk;
        });

        // The whole response has been received.
        response.on('end', () => {
            resolve();
        });
    }
}