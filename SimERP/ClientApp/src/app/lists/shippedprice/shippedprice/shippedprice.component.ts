import {Component, OnInit, ViewChild, ElementRef, AfterViewInit} from '@angular/core';
import {BsDatepickerConfig} from 'ngx-bootstrap';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {ToastrService} from 'ngx-toastr';
import {ShippedPrice} from '../model/shippedprice';
import {ShippedpriceService} from '../shippedprice.service';
import * as moment from 'moment';
import {PaginationComponent} from '../../../pagination/pagination.component';
import {LocalNumberPipe} from '../../../common/locale/pipes/local-number-pipe.service';
import {ComfirmDialogComponent} from '../../../common/comfirm-dialog/comfirm-dialog.component';
import {Globalfunctions} from '../../../common/globalfunctions/globalfunctions';
import {Key_DefaultPageLimit, Key_Delete_Icon, Key_Edit_Icon} from '../../../common/config/globalconfig';
import {StoreService} from '../../store/store.service';
import {Store} from '../../store/model/store';

@Component({
  selector: 'app-shippedprice',
  templateUrl: './shippedprice.component.html',
  styleUrls: ['./shippedprice.component.css']
})
export class ShippedpriceComponent implements OnInit, AfterViewInit {

  colorTheme = 'theme-blue';
  dataSerach: string;
  cboIsActive: number;
  lstDataResult: ShippedPrice[] = [];
  objModel: ShippedPrice;
  isNewModel: boolean;

  total = 10;
  page = 1;
  limit = Key_DefaultPageLimit;
  deleteIcon = Key_Delete_Icon;
  editIcon = Key_Edit_Icon;

  bsConfig: Partial<BsDatepickerConfig>;
  @ViewChild(PaginationComponent, {static: true}) pagingComponent: PaginationComponent;
  @ViewChild('closeAddExpenseModal', {static: true}) closeAddExpenseModal: ElementRef;
  lstStore: Store[] = [];

  constructor(private shippedpriceService: ShippedpriceService, private modalService: NgbModal, private toastr: ToastrService,
              private localNumberPipe: LocalNumberPipe, private storeService: StoreService) {

    this.objModel = new ShippedPrice();
    this.cboIsActive = -1;
    this.dataSerach = '';
  }

  ngOnInit() {
    this.bsConfig = Object.assign({}, {dateInputFormat: 'DD/MM/YYYY', containerClass: this.colorTheme, showWeekNumbers: false});
    this.storeService.getData('').subscribe(value => {
      if (value.IsOk) {
        this.lstStore = value.RepData;
      }
    });
  }

  ngAfterViewInit(): void {
    this.SearchData();
  }

  SerachAction() {
    this.page = 1;
    this.LoadData(0);
  }

  LoadData(startRow: number) {
    const limit = this.pagingComponent.getLimit();
    this.shippedpriceService.getData(this.dataSerach, startRow, limit).subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.lstDataResult = res.RepData;
            console.log(this.lstDataResult);
            this.total = res.TotalRow;
          }
        },
        error: (err) => {
          console.log(err);
        },
        complete: () => {

        }
      }
    );
  }

  AddModel() {
    this.isNewModel = true;
    this.objModel.CreatedDate = new Date();
  }

  EditModel(index: number) {
    this.isNewModel = false;
    this.objModel = Object.assign({}, this.lstDataResult[index]);
    this.objModel.CreatedDate = moment(this.objModel.CreatedDate, 'YYYY-MM-DDTHH:mm:ssZ').toDate();
    this.formatNumber();

  }

  saveDataModel(isclose: boolean) {
    if (!this.validateData()) {
      return;
    }
    this.shippedpriceService.Insert(this.objModel, this.isNewModel).subscribe(res => {
      if (res !== undefined) {
        if (!res.IsOk) {
          this.toastr.error(res.MessageText, 'Thông báo!');
        } else {
          this.SearchData();
          this.clearModel();
          this.toastr.success(this.isNewModel ? 'Thêm dữ liệu thành công' : 'Dữ liệu đã được chỉnh sửa', 'Thông báo!');
          if (isclose) {
            this.closeAddExpenseModal.nativeElement.click();
          } else {
            this.AddModel();
          }
        }
      } else {
        this.toastr.error('Lỗi xử lý hệ thống', 'Thông báo!');
      }
    }, err => {
      console.log(err);
    });
  }

  private validateData() {
    if (this.objModel.StoreId == null || this.objModel.StoreId == undefined || this.objModel.StoreId < 1) {
      this.toastr.info('Vui lòng chọn Kho vận chuyển!', 'Thông báo');
      return false;
    }
    return true;
  }

  clearModel() {
    this.objModel = new ShippedPrice();
  }

  CloseModel() {
    this.clearModel();
  }

  openDialog(Id: number) {
    const modalRef = this.modalService.open(ComfirmDialogComponent, {
      backdrop: false, scrollable: true, centered: true
    });
    // xử lý sau khi đóng dialog, thực hiện load lại dữ liệu nếu muốn
    modalRef.result.then((result) => {
      if (result !== undefined && result === true) {
        this.deleteRowGird(Id);
      }
    });
  }

  deleteRowGird(Id: number) {

    this.shippedpriceService.Delete(Id).subscribe(res => {
      if (res !== undefined) {
        if (!res.IsOk) {
          this.toastr.error(res.MessageText, 'Thông báo!');
        } else {
          this.toastr.warning('Dữ liệu đã được xóa', 'Thông báo!');
          const startRow = this.getStartRow();
          this.LoadData(startRow);
        }
      } else {
        this.toastr.error('Lỗi xử lý hệ thống', 'Thông báo!');
      }
    }, err => {
      console.log(err);
    });
  }

  actionUp(index: number) {
    if (index === 0) {
      return;
    }
    const objcusr: number = this.lstDataResult[index].Id;
    const objUp: number = this.lstDataResult[index - 1].Id;

    this.shippedpriceService.Sort(objcusr, objUp).subscribe(res => {
      if (res !== undefined) {
        if (!res.IsOk) {
          this.toastr.error(res.MessageText, 'Thông báo!');
        } else {
          this.SearchData();
        }
      } else {
        this.toastr.error('Lỗi xử lý hệ thống', 'Thông báo!');
      }
    }, err => {
      console.log(err);
    });

  }

  actionDow(index: number) {
    if (index === this.lstDataResult.length - 1) {
      return;
    }

    const objcusr: number = this.lstDataResult[index].Id;
    const objDow: number = this.lstDataResult[index + 1].Id;

    this.shippedpriceService.Sort(objDow, objcusr).subscribe(res => {
      if (res !== undefined) {
        if (!res.IsOk) {
          this.toastr.error(res.MessageText, 'Thông báo!');
        } else {
          this.SearchData();
        }
      } else {
        this.toastr.error('Lỗi xử lý hệ thống', 'Thông báo!');
      }
    }, err => {
      console.log(err);
    });
  }

  SearchData() {
    this.page = 1;
    this.LoadData(0);
  }

  getStartRow(): number {
    const startRow = (this.page - 1) * this.pagingComponent.getLimit();
    return startRow;
  }

  goToPage(n: number): void {
    this.page = n;
    const startRow = this.getStartRow();
    this.LoadData(startRow);
  }

  changeLimit() {
    this.page = 1;
    const startRow = this.getStartRow();
    this.LoadData(startRow);
  }

  onNext(): void {
    this.page++;
    const startRow = this.getStartRow();
    this.LoadData(startRow);
  }

  onPrev(): void {
    this.page--;
    const startRow = this.getStartRow();
    this.LoadData(startRow);
  }

  checkValidateForm() {
    if (this.objModel.MaDoGiaVc.length > 0 && this.objModel.GiaVc >= 0 && this.objModel.GiaVc != null) {
      return true;
    }
    return false;
  }

  formatNumber() {
    Globalfunctions.formatNumber('GiaVc', this.objModel.GiaVc);
  }

  isNumberKey(evt: any) {
    return Globalfunctions.isNumberKey(evt);
  }

}
