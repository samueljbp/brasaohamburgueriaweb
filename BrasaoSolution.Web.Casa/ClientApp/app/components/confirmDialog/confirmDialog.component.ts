import { Component, Input } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';

@Component({
    selector: 'confirm-dialog',
    templateUrl: './confirmDialog.component.html',
})
export class ConfirmationDialog {
    constructor(public dialogRef: MatDialogRef<ConfirmationDialog>) { }

    public confirmMessage: string;
}