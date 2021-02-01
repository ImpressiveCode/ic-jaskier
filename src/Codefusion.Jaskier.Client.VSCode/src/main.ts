'use strict';
import { ExtensionManager } from './core/extension-manager';
import { ExtensionContext } from 'vscode';

export function activate(context: ExtensionContext) {
    console.log('Jaskier: extension is active.');

    let manager = new ExtensionManager();
    manager.setup();
    manager.loadPredictions();

    // Add disposable subscriptions. When this extension is deactivated the disposables will be disposed.
    context.subscriptions.push(manager);
}

export function deactivate() {
    console.log('Jaskier: extension deactivation.');
}