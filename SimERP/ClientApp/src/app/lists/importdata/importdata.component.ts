import {Component, OnInit} from '@angular/core';
import {ImportdataService} from './importdata.service';
import {NotificationService} from '../../common/notifyservice/notification.service';
import {ComfirmDialogComponent} from '../../common/comfirm-dialog/comfirm-dialog.component';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-importdata',
  templateUrl: './importdata.component.html',
  styleUrls: ['./importdata.component.css']
})
export class ImportdataComponent implements OnInit {

  constructor(private service: ImportdataService, private notificationService: NotificationService, private modalService: NgbModal) {
  }

  ngOnInit() {
  }

  showConfirmImport() {
    const modalRef = this.modalService.open(ComfirmDialogComponent, {
      backdrop: false, scrollable: true, centered: true
    });

    modalRef.componentInstance.contentMessage = 'Bạn có chắc chắn muốn cập nhật dữ liệu hay không?';
    modalRef.result.then((result) => {
      if (result !== undefined && result === true) {
        this.importCIFPrice();
      }
    });
  }

  importCIFPrice() {
    this.service.importCIFPrice().subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.notificationService.showError(res.MessageText);
          } else {
            this.notificationService.showSucess('Cập nhật giá CIF thành công!');
          }
        },
        error: (err) => {
          console.log(err);
          this.notificationService.showError('Lỗi cập nhật thông tin giá CIF!');
        },
        complete: () => {
        }
      }
    );
  }
}
