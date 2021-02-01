import { Disposable, OutputChannel } from 'vscode';
import * as vscode from 'vscode';
import { Predictions } from '../predictions.model';
import { TelemetryService } from '../core/telemetry-service';

export class OutputChannelService implements Disposable {
    private outputChannel: OutputChannel;

    constructor(private telemetryService: TelemetryService) {
        this.outputChannel = vscode.window.createOutputChannel('Jaskier');
        this.outputChannel.appendLine('[Jaskier] Extension is active');
    }

    public showOutputChannel() {
        this.outputChannel.show();
        this.telemetryService.sendData('OUTPUT_CHANNEL_OPENED');
    }

    public appendMessages(...messages: string[]) {
        // Append empty line for clarity
        this.outputChannel.appendLine('');

        let messageHeader = `[Jaskier] ${this.formatDate(new Date())}`;
        this.outputChannel.appendLine(messageHeader);

        messages.forEach(loopMessage => {
            let messageWithTab = `\t${loopMessage}`;
            this.outputChannel.appendLine(messageWithTab);
        });
    }

    public dispose() {
        if (this.outputChannel) {
            this.outputChannel.dispose();
        }
    }

    private formatDate(date: Date): string {
        // 2018-03-05 09:17:20
        let year = date.getFullYear();
        let month = this.getWithPaddedZero(date.getMonth() + 1);
        let day = this.getWithPaddedZero(date.getDate());
        let hour = this.getWithPaddedZero(date.getHours());
        let minutes = this.getWithPaddedZero(date.getMinutes());
        let seconds = this.getWithPaddedZero(date.getSeconds());

        return `${year}-${month}-${day} ${hour}:${minutes}:${seconds}`;
    }

    private getWithPaddedZero(value: number): string {
        return ("0" + value).slice(-2);
    }
}