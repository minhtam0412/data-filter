import { Component, Input, OnInit } from '@angular/core';
import { ReportTotal } from '../model/reporttotal';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from 'src/app/common/notifyservice/notification.service';

@Component({
  selector: 'app-quantitytranform',
  templateUrl: './quantitytranform.component.html',
  styleUrls: ['./quantitytranform.component.css']
})
export class QuantitytranformComponent implements OnInit {

  @Input() lstDataResult: ReportTotal[] = [];

  lstDataFilter: ReportTotal[] = [];
  quychuankg: number = 1000;
  txtDvt: string = "";
  txtKeyword: string = "";
  txtQuydoi: string = "";

  constructor(private activeModal: NgbActiveModal, private notificationService: NotificationService) {

  }

  ngOnInit() {
    this.lstDataFilter = Object.assign([], this.lstDataResult);
    console.log(this.lstDataFilter);
  }

  quantitytranform() {
    this.lstDataResult.forEach(item => {
      if (this.removeAccents(item.Dvt) == "tan") {
        item.SoLuongQuyDoi = item.Luong * Number(this.quychuankg);
      }
      if (this.removeAccents(item.Dvt) == "kg") {
        item.SoLuongQuyDoi = item.Luong;
      }
    });
  }

  quantitytranforother() {
    if (this.txtQuydoi == null || this.txtQuydoi == "")
      return;
    if (this.txtDvt == null || this.txtDvt == "") {
      this.lstDataFilter.forEach(item => {
        let str = this.removeAccents(item.Dvt);
        if (str != "tan" && str != "kg") {
          item.SoLuongQuyDoi = Number(this.txtQuydoi) * item.Luong;
        }
      });
    }
    else {
      this.lstDataFilter.forEach(item => {
        let str_Dvt = this.removeAccents(item.Dvt);
        let str_txtDvt = this.removeAccents(this.txtDvt);
        if (str_Dvt.indexOf(str_txtDvt) != -1) {
          item.SoLuongQuyDoi = Number(this.txtQuydoi) * item.Luong;
        }
      });
    }

    this.lstDataFilter.forEach(item => {
      this.lstDataResult.find(x => x.id == item.id).SoLuongQuyDoi = item.SoLuongQuyDoi;
    });

  }

  removeAccents(str) {
    str = str.normalize("NFD").replace(/[\u0300-\u036f]/g, "").replace(/đ/g, "d").replace(/Đ/g, "D");
    return str.toLowerCase();
  }

  closeDialog() {
    this.activeModal.close(this.lstDataResult);
  }

  PreviewData() {
    this.filterDVT();
    this.filterKeyword();
  }

  filterDVT() {
    if (this.txtDvt == null || this.txtDvt == "")
      this.lstDataFilter = this.lstDataResult;
    let lsttem: ReportTotal[] = [];
    this.lstDataResult.forEach(item => {
      let str_Dvt = this.removeAccents(item.Dvt);
      let str_txtDvt = this.removeAccents(this.txtDvt);
      if (str_Dvt.indexOf(str_txtDvt) != -1) {
        lsttem.push(item);
      }
    });
    this.lstDataFilter = lsttem;
  }

  filterKeyword() {
    if (this.txtKeyword == null || this.txtKeyword == "")
      return;
    let lsttem: ReportTotal[] = [];

    this.lstDataFilter.forEach(item => {
      let str_Tenhang = this.removeAccents(item.ChungLoaiHangHoaXuatNhap).trim();
      let arr_str_Tenhang = str_Tenhang.split(' ');
      str_Tenhang = "";
      arr_str_Tenhang.forEach(str => {
        str_Tenhang += str;
      });

      let str_txtKeyword = this.removeAccents(this.txtKeyword).trim();
      let arr_str_txtKeyword = str_txtKeyword.split(' ');
      str_txtKeyword = "";
      arr_str_txtKeyword.forEach(str => {
        str_txtKeyword += str;
      });

      if (str_Tenhang.indexOf(str_txtKeyword) != -1) {
        lsttem.push(item);
      }
    });
    this.lstDataFilter = lsttem;
  }
}
