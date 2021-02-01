import * as fs from "fs";
import { ChangedFiles, ChangedFileInfo } from '../changed-files.model';
import * as vscode from 'vscode';
import * as path from 'path';
import * as ts from 'typescript';
import { IMetricsModel } from 'tsmetrics-core';
import { MetricsParser } from 'tsmetrics-core/MetricsParser';
import { MetricsConfiguration } from 'tsmetrics-core/MetricsConfiguration';

interface CyclomaticComplexityMetrics {
    maxComplexity: number;
    averageComplexity: number;
    medianComplexity: number;
}

export class CodeMetricsProvider {
    private metricsConfiguration = new MetricsConfiguration();
    private significantCodeElements = [
        this.metricsConfiguration.ConstructorDescription,
        this.metricsConfiguration.FunctionDeclarationDescription,
        this.metricsConfiguration.FunctionExpressionDescription,
        this.metricsConfiguration.MethodDeclarationDescription,
        this.metricsConfiguration.ArrowFunctionDescription
    ];
    private supportedFileExtensions = [
        '.ts',
        '.js'
    ]

    public loadCodeMetrics(data: ChangedFiles) {
        if (data.files.length === 0) {
            return;
        }

        data.files.forEach(item => this.loadCodeMetricsForFile(item));
    }

    private loadCodeMetricsForFile(file: ChangedFileInfo) {
        let fileExtension: string = path.extname(file.path);
        if (fileExtension == null || this.supportedFileExtensions.indexOf(fileExtension) === -1) {
            return;
        }

        try {
            let basePath = vscode.workspace.workspaceFolders[0].uri.fsPath;
            const filePath = path.join(basePath, file.path);
            const fileContent = fs.readFileSync(filePath, "utf8");

            let metrics = this.getMetrics(filePath, fileContent);

            file.maxCyclomaticComplexity = metrics.maxComplexity;
            file.averageCyclomaticComplexity = metrics.averageComplexity;
            file.medianCyclomaticComplexity = metrics.medianComplexity;
        }
        catch (error) {
            console.log("Error occurred: " + error);
        }
    }

    private getMetrics(filePath: string, fileContent: string): CyclomaticComplexityMetrics {
        let metricsForFile = MetricsParser.getMetricsFromText(filePath, fileContent, this.metricsConfiguration, ts.ScriptTarget.Latest);

        let significantComplexityMetrics: number[] = [];
        this.extractSignificantComplexityMetrics(metricsForFile.metrics, significantComplexityMetrics);

        return this.calculateOutputMetrics(significantComplexityMetrics);
    }

    private extractSignificantComplexityMetrics(metrics: IMetricsModel, significantComplexityMetrics: number[]) {
        if (this.isComplexityMetricSignificant(metrics)) {
            significantComplexityMetrics.push(metrics.getCollectedComplexity());
            return;
        }

        metrics.children.forEach(item => this.extractSignificantComplexityMetrics(item, significantComplexityMetrics));
    }

    private isComplexityMetricSignificant(model: IMetricsModel): boolean {
        return this.significantCodeElements.findIndex(item => item === model.description) >= 0;
    }

    private calculateOutputMetrics(complexityMetrics: number[]): CyclomaticComplexityMetrics {
        return <CyclomaticComplexityMetrics>{
            maxComplexity: complexityMetrics.length > 0 ? Math.max(...complexityMetrics) : null,
            averageComplexity: this.getAverage(complexityMetrics),
            medianComplexity: this.getMedian(complexityMetrics)
        };
    }

    private getAverage(values: number[]): number {
        if (values.length === 0) {
            return null;
        }

        let total = 0;
        values.forEach(item => {
            total += item;
        });

        return total / values.length;
    }

    private getMedian(values: number[]) {
        if (values.length === 0) {
            return null;
        }

        values.sort(function (a, b) {
            return a - b;
        });

        var half = Math.floor(values.length / 2);

        if (values.length % 2) {
            return values[half];
        } else {
            return (values[half - 1] + values[half]) / 2.0;
        }
    }
}