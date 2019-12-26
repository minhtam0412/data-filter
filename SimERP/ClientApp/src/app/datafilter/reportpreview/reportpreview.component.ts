import {Component, Input, OnInit} from '@angular/core';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';
import {NotificationService} from '../../common/notifyservice/notification.service';
import {ReportColumnView} from '../reportcolumnview/model/reportcolumnview';
import {ReportTotal} from '../model/reporttotal';
import {ReportcolumnviewService} from '../reportcolumnview/reportcolumnview.service';
import {ExportExcelService} from '../../common/exportAsExcel/export-excel.service';

@Component({
  selector: 'app-reportpreview',
  templateUrl: './reportpreview.component.html',
  styleUrls: ['./reportpreview.component.css']
})
export class ReportpreviewComponent implements OnInit {
  @Input() dropdownListOriginal: ReportColumnView[] = [];
  @Input() lstDataResult: ReportTotal[] = [];
  @Input() selectedItems: ReportColumnView[] = [];
  @Input() dropdownSettings = {};

  lstHeader: string[] = [];
  lstIndexActive: number[] = [];

  constructor(private activeModal: NgbActiveModal, private notificationService: NotificationService,
              private reportcolumnviewService: ReportcolumnviewService, private exportExcelService: ExportExcelService) {
  }

  ngOnInit() {
  }

  closeDialog() {
    this.activeModal.close(this.selectedItems);
  }

  export() {
    this.lstIndexActive.length = 0;
    this.dropdownListOriginal.forEach(value => {
      this.selectedItems.forEach(valueSelected => {
        if (value.ColumnCode === valueSelected.ColumnCode) {
          this.lstIndexActive.push(value.SortOrder);
        }
      });
    });
    this.dropdownListOriginal.forEach(value => {
      this.lstHeader.push(value.ColumnCode);
    });
    console.log(' this.lstDataResult', this.lstDataResult);
    console.log(' this.lstHeader', this.lstHeader);
    console.log(' this.lstIndexActive', this.lstIndexActive);
    this.exportExcelService.exportAsExcelFile(this.lstHeader, this.lstDataResult, this.lstIndexActive, 'SIM');
  }

  isAllChecked() {
    return this.lstDataResult.every(_ => _.Selected);
  }

  checkAll(ev) {
    this.lstDataResult.forEach(x => x.Selected = ev.target.checked);
  }

  checkSelected(colCode: string) {
    const index = this.selectedItems.findIndex(value => {
      return value.ColumnCode.toLowerCase() === colCode.toLowerCase();
    });
    return index > -1;
  }

  onChanged() {
    this.saveData(this.selectedItems);
  }

  saveData(lstItem: ReportColumnView[]) {
    this.reportcolumnviewService.saveData(lstItem).subscribe({
      next: (res) => {
        if (!res.IsOk) {
          this.notificationService.showError(res.MessageText, 'Thông báo');
        }
      },
      error: (err) => {
        console.log(err);
        this.notificationService.showError(err, 'Thông báo');
      }, complete: () => {
      }
    });
  }

}
