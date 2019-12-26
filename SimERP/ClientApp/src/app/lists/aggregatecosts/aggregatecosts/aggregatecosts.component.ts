import {Component, OnInit, ViewChild, ElementRef, AfterViewInit} from '@angular/core';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {ToastrService} from 'ngx-toastr';
import {ComfirmDialogComponent} from '../../../common/comfirm-dialog/comfirm-dialog.component';
import {AggregateCosts} from '../model/aggregatecosts';
import {AggregatecostsService} from '../aggregatecosts.service';
import {BsDatepickerConfig} from 'ngx-bootstrap';
import * as moment from 'moment';
import {Key_DefaultPageLimit, Key_Delete_Icon, Key_Edit_Icon} from '../../../common/config/globalconfig';
import {LocalNumberPipe} from '../../../common/locale/pipes/local-number-pipe.service';
import {Globalfunctions} from '../../../common/globalfunctions/globalfunctions';
import {PaginationComponent} from '../../../pagination/pagination.component';

declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-aggregatecosts',
  templateUrl: './aggregatecosts.component.html',
  styleUrls: ['./aggregatecosts.component.css']
})
export class AggregatecostsComponent implements OnInit, AfterViewInit {

  colorTheme = 'theme-blue';
  dataSerach: string;
  cboIsActive: number;
  lstDataResult: AggregateCosts[] = [];
  objModel: AggregateCosts;
  isNewModel: boolean;

  total = 10;
  page = 1;
  limit = Key_DefaultPageLimit;
  deleteIcon = Key_Delete_Icon;
  editIcon = Key_Edit_Icon;

  bsConfig: Partial<BsDatepickerConfig>;
  @ViewChild(PaginationComponent, {static: true}) pagingComponent: PaginationComponent;
  @ViewChild('closeAddExpenseModal', {static: true}) closeAddExpenseModal: ElementRef;

  constructor(private aggregatecostsService: AggregatecostsService, private modalService: NgbModal, private toastr: ToastrService,
              private localNumberPipe: LocalNumberPipe) {

    this.objModel = new AggregateCosts();
    this.cboIsActive = -1;
    this.dataSerach = '';
  }

  ngOnInit() {
    this.bsConfig = Object.assign({}, {dateInputFormat: 'DD/MM/YYYY', containerClass: this.colorTheme, showWeekNumbers: false});
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
    this.aggregatecostsService.getData(this.dataSerach, startRow, limit).subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.lstDataResult = res.RepData;
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
    this.objModel.ApplyDate = new Date();
  }

  EditModel(index: number) {
    this.isNewModel = false;
    this.objModel = Object.assign({}, this.lstDataResult[index]);
    this.objModel.ApplyDate = moment(this.objModel.ApplyDate, 'YYYY-MM-DDTHH:mm:ssZ').toDate();
    this.formatNumber();

  }

  saveDataModel(isclose: boolean) {
    this.aggregatecostsService.Insert(this.objModel, this.isNewModel).subscribe(res => {
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

  clearModel() {
    this.objModel = new AggregateCosts();
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

    this.aggregatecostsService.Delete(Id).subscribe(res => {
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

    this.aggregatecostsService.Sort(objcusr, objUp).subscribe(res => {
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

    this.aggregatecostsService.Sort(objDow, objcusr).subscribe(res => {
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
    if (this.objModel.LoaiCp.length > 0 && this.objModel.ChiPhi >= 0 && this.objModel.ChiPhi != null && this.objModel.ApplyDate != null) {
      return true;
    }
    return false;
  }

  formatNumber() {
    Globalfunctions.formatNumber('ChiPhi', this.objModel.ChiPhi, '.2-2');
  }

  isNumberKey(evt: any) {
    return Globalfunctions.isNumberKey(evt);
  }
}
