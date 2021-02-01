import { ChangesTracker, ChangedFiles, ChangedFileInfo } from '../changed-files.model';
import * as Git from 'simple-git/promise';
import * as GitRaw from 'simple-git';

export class GitChangesTracker implements ChangesTracker {
    private authorName: string;

    constructor(private repositoryPath: string) {
        this.setAuthorName(repositoryPath);
    }

    public async getChangedFiles(): Promise<ChangedFiles> {
        let repo = Git(this.repositoryPath);
        let diffSummary = await repo.diffSummary();

        return this.convertDiffSummary(diffSummary);
    }

    private convertDiffSummary(diffSummary: Git.DiffResult): ChangedFiles {
        if (!diffSummary || !diffSummary.files) {
            return null;
        }

        let changedFiles = diffSummary.files.map(loopFile => {
            // Backend service requires paths with backslashes as separators.
            let path = loopFile.file.replace(new RegExp('/', 'g'), '\\', );

            return <ChangedFileInfo>{
                author: this.authorName,
                path: path,
                oldPath: path,
                numberOfModifiedLines: loopFile.changes
            };
        });

        return new ChangedFiles(changedFiles);
    }

    private setAuthorName(repositoryPath: string) {
        let repo = GitRaw(this.repositoryPath);
        repo.raw([
            'config',
            'user.name'
        ], (error, result) => {
            if (error) {
                console.error('Error occurred while reading user.name from config. Error: ' + error);
                this.authorName = 'Unknown';
            } else {
                this.authorName = result.trim();
            }
        });
    }
}