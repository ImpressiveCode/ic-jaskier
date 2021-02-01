import * as vscode from 'vscode';

type FileChangeHandlerFn = (fileUri: vscode.Uri) => void;

interface PendingEvent {
    fileUri: vscode.Uri;
    handler: FileChangeHandlerFn;
}

export class FileWatcher implements vscode.Disposable {
    private watcher: vscode.FileSystemWatcher;

    private suspendEvents = false;
    private pendingEvent: PendingEvent = null;
    private readonly throttlingInterval = 5000; // ms
    private throttlingTimer: NodeJS.Timer = null;

    // All files except:
    // - files starting with a dot.
    // - files located in a folder with name starting with a dot (e.g. '.git/*.*')
    private readonly globPatternForFiles = '**/[^.]*/[^.]*.*';

    public setup(onChangeFn: FileChangeHandlerFn) {
        // vscode Watcher can watch files from the currently opened workspace.
        this.watcher = vscode.workspace.createFileSystemWatcher(this.globPatternForFiles);

        this.watcher.onDidChange(uri => this.handleFileChange(uri, onChangeFn));
        this.watcher.onDidCreate(uri => this.handleFileChange(uri, onChangeFn));
        this.watcher.onDidDelete(uri => this.handleFileChange(uri, onChangeFn));
    }

    public dispose() {
        if (this.watcher) {
            this.watcher.dispose();
        }

        if (this.throttlingTimer) {
            clearTimeout(this.throttlingTimer);
        }
    }

    private handleFileChange(fileUri: vscode.Uri, handler: FileChangeHandlerFn) {
        if (this.suspendEvents) {
            this.pendingEvent = { fileUri, handler };
            return;
        }

        this.setupThrottling();
        this.pendingEvent = null;

        console.log(`FileChanged: ${fileUri.toString()}`);
        if (handler) {
            handler(fileUri);
        }
    }

    private setupThrottling() {
        this.suspendEvents = true;

        this.throttlingTimer = setTimeout(
            () => {
                this.suspendEvents = false;
                this.throttlingTimer = null;
                this.handlePendingEvent();
            },
            this.throttlingInterval);
    }

    private handlePendingEvent() {
        if (this.pendingEvent != null) {
            this.handleFileChange(this.pendingEvent.fileUri, this.pendingEvent.handler);
        }
    }
}