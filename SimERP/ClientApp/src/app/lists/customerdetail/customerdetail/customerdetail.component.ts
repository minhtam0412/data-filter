import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CustomerDetail } from '../model/customerdetail';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { CustomerdetailService } from '../customerdetail.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { AreaList } from '../../area/model/arealist';
import { AreaService } from '../../area/area.service';
import { CustomerDeliveryAddress } from '../model/customerdeliveryaddress';
import { PaginationComponent } from '../../../pagination/pagination.component';
import { Key_DefaultPageLimit, Key_Delete_Icon, Key_Edit_Icon, Key_MaxRow } from '../../../common/config/globalconfig';
import { LocalNumberPipe } from '../../../common/locale/pipes/local-number-pipe.service';
import { ComfirmDialogComponent } from '../../../common/comfirm-dialog/comfirm-dialog.component';
import { isUndefined } from 'ngx-bootstrap/chronos/utils/type-checks';
import { Globalfunctions } from '../../../common/globalfunctions/globalfunctions';
import { Sales } from '../model/Sales';
import { ListCurrency, ListPayType } from 'src/app/common/masterdata/commondata';
import { ProductDetail } from '../../product/model/productdetail';
import * as moment from 'moment';

declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-customerdetail',
  templateUrl: './customerdetail.component.html',
  styleUrls: ['./customerdetail.component.css']
})
export class CustomerdetailComponent implements OnInit, AfterViewInit {

  constructor(private customerdetailService: CustomerdetailService, private modalService: NgbModal, private toastr: ToastrService,
    private localNumberPipe: LocalNumberPipe, private areaService: AreaService) {

    this.objModel = new CustomerDetail();
    this.cboIsActive = -1;
    this.dataSerach = '';
    this.cboAreaList = -1;
  }

  colorTheme = 'theme-blue';
  dataSerach: string;
  cboIsActive: number;
  objModel: CustomerDetail;
  isNewModel: boolean;
  cboAreaList: number;

  lstDataResult: CustomerDetail[] = [];
  lstAreaList: AreaList[] = [];
  lstProductDetail: ProductDetail[] = [];

  total = 10;
  page = 1;
  limit = Key_DefaultPageLimit;
  deleteIcon = Key_Delete_Icon;
  editIcon = Key_Edit_Icon;
  bsConfig: Partial<BsDatepickerConfig>;
  lstCurrencyType = ListCurrency;
  ListPayType = ListPayType;

  @ViewChild(PaginationComponent, { static: false })
  pagingComponent: PaginationComponent;
  @ViewChild('closeAddExpenseModal', { static: true }) closeAddExpenseModal: ElementRef;

  ngOnInit() {
    this.bsConfig = Object.assign({}, { dateInputFormat: 'DD/MM/YYYY', containerClass: this.colorTheme, showWeekNumbers: false });
    this.loadCboAreaList();
    this.loadCboProduct();
  }

  ngAfterViewInit(): void {
    this.SearchData();
  }

  SerachAction() {
    this.page = 1;
    this.LoadData(0);
  }

  loadCboAreaList() {
    this.areaService.getData('', 1, 0, Key_MaxRow).subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.lstAreaList = res.RepData;
          }
        },
        error: (err) => {
          console.log(err);
        }
      }
    );
  }

  loadCboProduct() {
    this.customerdetailService.GetProductCache().subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.lstProductDetail = res.RepData;

          }
        },
        error: (err) => {
          console.log(err);
        }
      }
    );
  }

  LoadData(startRow: number) {
    const limit = this.pagingComponent.getLimit();
    this.customerdetailService.getData(this.dataSerach, this.cboAreaList, startRow, limit).subscribe(
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
    this.objModel.CreatedDate = new Date();
  }

  EditModel(index: number) {
    this.isNewModel = false;
    this.objModel = Object.assign({}, this.lstDataResult[index]);
    this.objModel.lstSales.forEach(value => {
      value.ThoiGianBan = moment(value.ThoiGianBan, 'DD-MM-YYYYTHH:mm:ssZ').toDate();
    });
  }

  saveDataModel(isclose: boolean) {
    if (!this.validateData()) {
      return;
    }

    this.customerdetailService.Insert(this.objModel, this.isNewModel).subscribe(res => {
      if (res !== undefined) {
        if (!res.IsOk) {
          this.toastr.error(res.MessageText, 'Thông báo!');
        } else {
          const startRow = this.getStartRow();
          this.LoadData(startRow);
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
    this.objModel = new CustomerDetail();
    const form = $('#formModel')[0];
    $(form).removeClass('is-invalid');
    $(form).removeClass('was-validated');
    // form.reset();
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

    this.customerdetailService.Delete(Id).subscribe(res => {
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

  deleteRowAddress(index: number) {
    const modalRef = this.modalService.open(ComfirmDialogComponent, {
      backdrop: false, scrollable: true, centered: true
    });
    // xử lý sau khi đóng dialog, thực hiện load lại dữ liệu nếu muốn
    modalRef.result.then((result) => {
      if (result !== undefined && result === true) {
        this.objModel.lstCustomerDeliveryAddress.splice(index, 1);
      }
    });
  }

  deleteRowSales(index: number) {
    const modalRef = this.modalService.open(ComfirmDialogComponent, {
      backdrop: false, scrollable: true, centered: true
    });
    // xử lý sau khi đóng dialog, thực hiện load lại dữ liệu nếu muốn
    modalRef.result.then((result) => {
      if (result !== undefined && result === true) {
        this.objModel.lstSales.splice(index, 1);
      }
    });
  }

  actionUp(index: number) {
    if (index === 0) {
      return;
    }
    const objcusr: number = this.lstDataResult[index].CusId;
    const objUp: number = this.lstDataResult[index - 1].CusId;

    this.customerdetailService.Sort(objcusr, objUp).subscribe(res => {
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

    const objcusr: number = this.lstDataResult[index].CusId;
    const objDow: number = this.lstDataResult[index + 1].CusId;

    this.customerdetailService.Sort(objDow, objcusr).subscribe(res => {
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
    if (this.objModel.CustomerCode != null && this.objModel.CustomerCode.length > 0 && this.objModel.NameCus.length > 0
      && this.objModel.FullnameCus.length > 0) {
      return true;
    }
    return false;
  }

  clearustomerType() {
    this.cboAreaList = -1;
  }

  AddCustomerDeliveryAddress() {
    const item: CustomerDeliveryAddress = new CustomerDeliveryAddress();
    item.SortOrder = this.getMaxSortOrder() + 1;
    if (this.objModel.lstCustomerDeliveryAddress.length === 0) {
      item.IsDefault = true;
    }
    this.objModel.lstCustomerDeliveryAddress.push(item);
  }

  AddSales() {
    const item: Sales = new Sales();
    this.objModel.lstSales.push(item);
  }

  // Get max Sort Order from List
  getMaxSortOrder() {
    let maxSortOrder = Math.max.apply(Math, this.objModel.lstCustomerDeliveryAddress.map(function (value) {
      return value.SortOrder;
    }));
    if (maxSortOrder === -Infinity) {
      maxSortOrder = 0;
    }
    return maxSortOrder;
  }

  public autocompleteHeaderTemplate = `
  <div class="header-row">
    <div class="col-1">ID</div>
    <div class="col-3">Mã khu vực</div>
    <div class="col-6">Tên khu vực</div>
  </div>`;

  public renderDataRowAutoComplete(data: AreaList): string {
    const html = `
      <div class="data-row">
        <div class="col-1">${data.Id}</div>
        <div class="col-3">${data.MaKhuVuc}</div>
        <div class="col-6">${data.TenKhuVuc}</div>
      </div>`;
    return html;
  }

  autocompleteCallback(event, index: number) {
    console.log(event, index);
    const item: AreaList = this.lstAreaList.find(x => x.Id === event);
    if (item !== undefined) {
      this.objModel.lstCustomerDeliveryAddress[index].IdKhuVuc = item.Id;
      this.objModel.lstCustomerDeliveryAddress[index].MaKhuVuc = item.MaKhuVuc;
      this.objModel.lstCustomerDeliveryAddress[index].TenKhuVuc = item.TenKhuVuc;
    }
  }

  onKeydown(event) {
    if (event.key === 'Enter') {
    }
  }

  //-----------Lịch sử bán-----------------
  public sales_autocompleteHeaderTemplate = `
    <div class="header-row">
      <div class="col-3">Mã hàng hóa</div>
      <div class="col-3">Tên hàng hóa</div>
      <div class="col-5">Tên hàng hóa đầy đủ</div>
    </div>`;

  public sales_renderDataRowAutoComplete(data: ProductDetail): string {
    const html = `
        <div class="data-row">
        <div class="col-3 text-left">${data.ProductCode == null ? "" : data.ProductCode}</div>
        <div class="col-3 text-left">${data.ProductName == null ? "" : data.ProductName}</div>
        <div class="col-5">${data.ProductNameFull == null ? "" : data.ProductNameFull}</div>
      </div>`;
    return html;
  }

  sales_autocompleteCallback(event, index: number) {
    console.log(event, index);
    const item: ProductDetail = this.lstProductDetail.find(x => x.Id === event);
    if (item !== undefined) {
      this.objModel.lstSales[index].ProductId = item.ProductId;
      this.objModel.lstSales[index].ProductCode = item.ProductCode;
      this.objModel.lstSales[index].ProductName = item.ProductName;
    }
  }

  sales_onKeydown(event) {
    if (event.key === 'Enter') {
    }
  }
  //-------------------------
  Uppercase(value) {
    this.objModel.CustomerCode = String(value).toLocaleUpperCase();
  }

  checkIsDefault(index: number) {
    const valueOriginal = this.objModel.lstCustomerDeliveryAddress[index].IsDefault;
    this.objModel.lstCustomerDeliveryAddress.forEach(item => {
      item.IsDefault = false;
    });
    this.objModel.lstCustomerDeliveryAddress[index].IsDefault = valueOriginal ? true : false;
  }

  validateData(): boolean {
    if (this.objModel.lstCustomerDeliveryAddress.length === 0) {
      this.toastr.info('Vui lòng nhập ít nhất 1 Khu vực giao hàng!');
      return false;
    }

    let isInputFullDataRow = true;
    this.objModel.lstCustomerDeliveryAddress.forEach(value => {
      if (value.DeliveryAdr.length === 0 || value.MaKhuVuc.length === 0) {
        isInputFullDataRow = false;
        return;
      }
    });
    if (!isInputFullDataRow) {
      this.toastr.info('Vui lòng nhập đầy đủ dữ liệu khu vực giao hàng!');
      return false;
    }

    const checkHasDefault = this.objModel.lstCustomerDeliveryAddress.every(value => {
      return value.IsDefault === false;
    });
    if (checkHasDefault) {
      this.toastr.info('Vui lòng chọn khu vực mặc định!');
      return false;
    }

    const deliveryAddressDefault = this.objModel.lstCustomerDeliveryAddress.find(value => {
      return value.IsDefault === true;
    });

    // chuẩn hoá các dữ liệu để hiển thị ngoài danh sách
    if (deliveryAddressDefault) {
      this.objModel.DeliveryAdr = deliveryAddressDefault.DeliveryAdr;
    }

    // lịch sử bán hàng
    let isInputRowSales = true;
    this.objModel.lstSales.forEach(value => {
      if (value.ProductId === undefined || value.ProductId === null ||  value.ProductCode.length === 0) {
        isInputRowSales = false;
        return;
      }
    });
    if (!isInputRowSales) {
      this.toastr.info('Vui lòng nhập đầy đủ dữ liệu lịch sử bán hàng!');
      return false;
    }

    return true;
  }

  isNumberKey(evt: any) {
    return Globalfunctions.isNumberKey(evt);
  }

  clearLoaiTien() {
  }

  clearLoaiThanhToan() {
  }

  changeEffectiveDate(value: Date, row: Sales) {
    const dateNow = new Date();
    row.ThoiGianBan.setHours(dateNow.getHours());
    row.ThoiGianBan.setMinutes(dateNow.getMinutes());
    row.ThoiGianBan.setMilliseconds(dateNow.getMilliseconds());
  }
}
