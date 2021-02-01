export interface ChangesTracker {
    getChangedFiles(): Promise<ChangedFiles>;
}

export interface ChangedFileInfo {
    author: string;
    path: string;
    oldPath: string;
    numberOfModifiedLines: number;
    maxCyclomaticComplexity?: number;
    averageCyclomaticComplexity?: number;
    medianCyclomaticComplexity?: number;
}

export class ChangedFiles {
    constructor(public files: ChangedFileInfo[]) {
        this.files.sort((a, b) => this.sortByPath(a, b));
    }

    public equals(other: ChangedFiles): boolean {
        if (other == null || other.files == null ||
            other.files.length !== this.files.length) {

            return false;
        }

        let areEqual = true;

        for (let loopIndex = 0; loopIndex < this.files.length; loopIndex++) {
            let item1 = this.files[loopIndex];
            let item2 = other.files[loopIndex];

            if (!this.areItemsEqual(item1, item2)) {
                areEqual = false;
                break;
            }
        }

        return areEqual;
    }

    private sortByPath(item1: ChangedFileInfo, item2: ChangedFileInfo): number {
        return item1.path.localeCompare(item2.path);
    }

    private areItemsEqual(item1: ChangedFileInfo, item2: ChangedFileInfo): boolean {
        // NOTE: Cyclomatic complexity properties are ignored in this comparison.
        if (item1.path === item2.path &&
            item1.oldPath === item2.oldPath &&
            item1.author === item2.author &&
            item1.numberOfModifiedLines === item2.numberOfModifiedLines) {

            return true;
        }

        return false;
    }
}
