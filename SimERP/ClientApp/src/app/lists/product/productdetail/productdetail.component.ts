import {AfterViewChecked, Component, Input, OnInit} from '@angular/core';
import {NgbActiveModal, NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {NotificationService} from '../../../common/notifyservice/notification.service';
import {ProductDetail} from '../model/productdetail';
import {ProductService} from '../product.service';
import {ProductCIFPrice} from '../model/productcifprice';
import {Key_Delete_Icon, Key_Theme_BsDatePicker} from '../../../common/config/globalconfig';
import {ListCurrency} from '../../../common/masterdata/commondata';
import {ComfirmDialogComponent} from '../../../common/comfirm-dialog/comfirm-dialog.component';
import * as moment from 'moment';
import {Globalfunctions} from '../../../common/globalfunctions/globalfunctions';
import {CustomerDetail} from '../../customerdetail/model/customerdetail';

@Component({
  selector: 'app-productdetail',
  templateUrl: './productdetail.component.html',
  styleUrls: ['./productdetail.component.css']
})
export class ProductdetailComponent implements OnInit, AfterViewChecked {

  @Input() isAddState = true;
  @Input() rowSelected: ProductDetail;
  private resultCloseDialog = false;
  model: ProductDetail = new ProductDetail();
  colorTheme = Key_Theme_BsDatePicker;
  lstCurrencyType = ListCurrency;
  @Input() lstCustomer: CustomerDetail[] = [];
  public customerAutocompleteHeaderTemplate = `
    <div class="header-row">
      <div class="col-4">Tên khách hàng</div>
      <div class="col-5">Tên khách hàng đầy đủ</div>
    </div>`;
  deleteIcon = Key_Delete_Icon;

  constructor(private activeModal: NgbActiveModal, private notificationService: NotificationService,
              private service: ProductService, private modalService: NgbModal) {
  }

  ngOnInit() {
    if (this.isAddState) {
      this.model = new ProductDetail();
    } else {
      this.service.getInfo(this.rowSelected.Id).subscribe({
        next: res => {
          if (res && res.IsOk) {
            this.model = res.RepData;
            this.model.ListProductCIFPrice.forEach(value => {
              if (value.EffectiveDate != null) {
                value.EffectiveDate = moment(value.EffectiveDate, 'YYYY-MM-DDTHH:mm:ssZ').toDate();
              }
              if (value.EffectiveDateNorth != null) {
                value.EffectiveDateNorth = moment(value.EffectiveDateNorth, 'YYYY-MM-DDTHH:mm:ssZ').toDate();
              }
            });
          } else {
            console.log(res);
            this.notificationService.showError(res.MessageText);
          }
        }, error: err => {
          console.log(err);
          this.notificationService.showError('Lỗi load thông tin!');
        }
        , complete: () => {

        }
      });
    }
  }

  closeDialog() {
    this.activeModal.close(this.resultCloseDialog);
  }

  getActionText() {
    return this.isAddState ? 'Thêm mới ' : 'Cập nhật ';
  }

  saveData(isContinue: boolean) {
    if (!this.validateData()) {
      return;
    }
    console.log(JSON.stringify(this.model));
    if (this.isAddState) {
      this.model.CreatedDate = new Date();
    } else {
      this.model.ModifyDate = new Date();
    }
    this.service.saveData(this.model, this.isAddState).subscribe({
      next: (res) => {
        if (res.IsOk) {
          this.notificationService.showSucess(this.getActionText() + 'thành công');
          this.resultCloseDialog = true;
          if (this.isAddState) {
            if (isContinue) {
              this.model = new ProductDetail();
            } else {
              this.closeDialog();
            }
          } else {
            if (isContinue) {
              this.isAddState = true;
              this.model = new ProductDetail();
            } else {
              this.closeDialog();
            }
          }
        } else {
          this.notificationService.showError(res.MessageText, 'Thông báo');
        }
      },
      error: (err) => {
        console.log(err);
        this.notificationService.showError(err, 'Thông báo');
        this.resultCloseDialog = false;
      }, complete: () => {
      }
    });
  }

  showConfirmDeleteDialog(row: ProductCIFPrice) {
    const modalRef = this.modalService.open(ComfirmDialogComponent, {
      backdrop: false, scrollable: true, centered: true
    });

    modalRef.result.then((result) => {
      if (result !== undefined && result === true) {
        this.deleteCIFPrice(row);
      }
    });
  }

  addCIFPrice() {
    if (this.model === undefined) {
      this.model = new ProductDetail();
    }

    const productCIFPrice = new ProductCIFPrice();
    productCIFPrice.SortOrder = this.getMaxSortOrder() + 1;
    let countIsDefaultSouth = 0;
    let countIsDefaultNorth = 0;
    this.model.ListProductCIFPrice.forEach(value => {
      if (value.IsDefault === 1) {
        countIsDefaultSouth++;
      }
      if (value.IsDefaultNorth === 1) {
        countIsDefaultNorth++;
      }
    });

    if (countIsDefaultSouth === 0) {
      productCIFPrice.IsDefault = 1;
    }
    if (countIsDefaultNorth === 0 && countIsDefaultSouth === 1) {
      productCIFPrice.IsDefaultNorth = 1;
    }
    this.model.ListProductCIFPrice.push(productCIFPrice);
  }

  clearLoaiTien() {

  }

  // Get max Sort Order from List
  getMaxSortOrder() {
    let maxSortOrder = Math.max.apply(Math, this.model.ListProductCIFPrice.map(function (value) {
      return value.SortOrder;
    }));
    if (maxSortOrder === -Infinity) {
      maxSortOrder = 0;
    }
    return maxSortOrder;
  }

  private deleteCIFPrice(row: ProductCIFPrice) {
    if (row.Id > 0) {
      if (this.model.ListProductCIFPricsDel === undefined || this.model.ListProductCIFPricsDel == null) {
        this.model.ListProductCIFPricsDel = [];
      }
      this.model.ListProductCIFPricsDel.push(row);
    }

    const index = this.model.ListProductCIFPrice.findIndex((item) => {
      return item.EffectiveDate === row.EffectiveDate && item.LoaiTien === row.LoaiTien;
    });

    this.model.ListProductCIFPrice.splice(index, 1);
  }


  checkIsDefault(index: number) {
    const valueOriginal = this.model.ListProductCIFPrice[index].IsDefault;
    this.model.ListProductCIFPrice.forEach(item => {
      item.IsDefault = 0;
    });
    this.model.ListProductCIFPrice[index].IsDefault = valueOriginal ? 1 : 0;
  }

  isNumberKey(evt: any) {
    return Globalfunctions.isNumberKey(evt);
  }

  formatNumber() {
    Globalfunctions.formatNumber('CpveKhoBn', this.model.CpveKhoBn);
    Globalfunctions.formatNumber('CpveKhoLh', this.model.CpveKhoLh);
    Globalfunctions.formatNumber('GiaMin', this.model.GiaMin);
    Globalfunctions.formatNumber('GiaMax', this.model.GiaMax);
    Globalfunctions.formatNumber('GiaTb', this.model.GiaTb);
    this.model.ListProductCIFPrice.forEach((value, index) => {
      Globalfunctions.formatNumber('CIFSouth' + String(index), value.Cifsouth, '.2-2');
      Globalfunctions.formatNumber('CIFNorth' + String(index), value.Cifnorth, '.2-2');
    });
  }

  ngAfterViewChecked(): void {
  }

  validateData(): boolean {
    if ((this.model.GiaMin !== undefined && this.model.GiaMin != null) || (this.model.GiaMax !== undefined && this.model.GiaMax != null)
      || (this.model.GiaTb !== undefined && this.model.GiaTb != null)) {
      if (this.model.GiaMin === undefined || this.model.GiaMin == null || String(this.model.GiaMin).length === 0) {
        this.notificationService.showInfo('Vui lòng nhập giá bán Min!');
        return false;
      }
      if (this.model.GiaMax === undefined || this.model.GiaMax == null || String(this.model.GiaMax).length === 0) {
        this.notificationService.showInfo('Vui lòng nhập giá bán Max!');
        return false;
      }
      if (this.model.GiaTb === undefined || this.model.GiaTb == null || String(this.model.GiaTb).length === 0) {
        this.notificationService.showInfo('Vui lòng nhập giá bán Bình quân!');
        return false;
      }
    }

    if (this.model.GiaMin > this.model.GiaMax) {
      this.notificationService.showInfo('Giá Min không được lớn hơn giá Max!');
      return false;
    }

    if (this.model.GiaMin > this.model.GiaTb) {
      this.notificationService.showInfo('Giá Min không được lớn hơn giá Bình quân!');
      return false;
    }

    if (this.model.GiaTb > this.model.GiaMax) {
      this.notificationService.showInfo('Giá Bình quân không được lớn hơn giá Max!');
      return false;
    }

    if (this.model.ListProductCIFPrice.length === 0) {
      this.notificationService.showInfo('Vui lòng nhập ít nhất 1 giá CIF!');
      return false;
    }

    let isInputFullDataRow = true;
    let message = '';
    this.model.ListProductCIFPrice.forEach((item, index) => {
      if (item.Cifsouth == null && item.LoaiTien == null && item.EffectiveDate == null &&
        item.Cifnorth == null && item.LoaiTienNorth == null && item.EffectiveDateNorth == null) {
        message = 'Vui lòng nhập đầy đủ dữ liệu giá CIF của sản phẩm!';
        isInputFullDataRow = false;
        return;
      }

      // validate cho Kho Long Hậu
      if ((item.Cifsouth !== undefined && item.Cifsouth !== null && item.Cifsouth !== 0) || (item.LoaiTien !== undefined
        && item.LoaiTien !== null && item.LoaiTien.length > 0) || (item.EffectiveDate !== undefined && item.EffectiveDate !== null)) {
        if (item.Cifsouth == null) {
          message = 'Vui lòng nhập giá CIF cho Kho Long Hậu!';
          isInputFullDataRow = false;
          return;
        }
        if (item.LoaiTien == null) {
          message = 'Vui chọn loại tiền cho Kho Long Hậu!';
          isInputFullDataRow = false;
          return;
        }
        if (item.EffectiveDate == null) {
          message = 'Vui chọn ngày tờ khai cho Kho Long Hậu!';
          isInputFullDataRow = false;
          return;
        }
      }
      // validate cho Kho Bắc Ninh
      if ((item.Cifnorth !== undefined && item.Cifnorth !== null && item.Cifnorth !== 0) || (item.LoaiTienNorth !== undefined
        && item.LoaiTienNorth !== null && item.LoaiTienNorth.length > 0) || (item.EffectiveDateNorth !== undefined &&
        item.EffectiveDateNorth !== null)) {
        if (item.Cifnorth == null) {
          message = 'Vui lòng nhập giá CIF cho Kho Bắc Ninh!';
          isInputFullDataRow = false;
          return;
        }
        if (item.LoaiTienNorth == null) {
          message = 'Vui chọn loại tiền cho Kho Bắc Ninh!';
          isInputFullDataRow = false;
          return;
        }
        if (item.EffectiveDateNorth == null) {
          console.log('index: ' + index);
          console.log(item);
          message = 'Vui chọn ngày tờ khai cho Kho Bắc Ninh!';
          isInputFullDataRow = false;
          return;
        }
      }
    });
    if (!isInputFullDataRow) {
      this.notificationService.showInfo(message);
      return false;
    }

    const checkHasDefaultSouth = this.model.ListProductCIFPrice.every(value => {
      return value.IsDefault === 0;
    });
    const checkHasDefaultNorth = this.model.ListProductCIFPrice.every(value => {
      return value.IsDefaultNorth === 0;
    });
    if (checkHasDefaultSouth && checkHasDefaultNorth) {
      this.notificationService.showInfo('Vui lòng chọn ít nhất 1 giá hiện hành!');
      return false;
    }

    // chuẩn hoá các dữ liệu Giá CIF, Loại tiền cho table ProductDetail
    // khi sử dụng thì không cần lọc lại dữ liệu từ table ProductCIFPrice
    const currentSouth = this.model.ListProductCIFPrice.find(value => {
      return value.IsDefault === 1;
    });
    if (currentSouth != null) {
      this.model.GiaCif = currentSouth.Cifsouth;
      this.model.LoaiTien = currentSouth.LoaiTien;
    } else {
      this.model.GiaCif = 0;
      this.model.LoaiTien = null;
    }

    const currentNorth = this.model.ListProductCIFPrice.find(value => {
      return value.IsDefaultNorth === 1;
    });
    if (currentNorth != null) {
      this.model.GiaCifnorth = currentNorth.Cifnorth;
      this.model.LoaiTienNorth = currentNorth.LoaiTienNorth;
    } else {
      this.model.GiaCifnorth = 0;
      this.model.LoaiTienNorth = null;
    }

    return true;
  }

  uppercase(value) {
    this.model.ProductCode = String(value).toLocaleUpperCase();
  }

  // xử lý render data dropdown
  public customerRenderDataRowAutoComplete(data: CustomerDetail): string {
    const html = `
      <div class="data-row">
        <div class="col-4">${data.NameCus}</div>
        <div class="col-5">${data.FullnameCus}</div>
      </div>`;
    return html;
  }

  // xử lý sau khi chọn khách hàng, gán ngầm Id khách hàng
  customerMinAutocompleteCallback(value) {
    const inputValue = this.model.KhMinName;
    const customerDetail: CustomerDetail = Object.assign({}, this.lstCustomer.find(x => x.CusId === value));
    if (customerDetail != null && customerDetail.CusId > 0) {
      this.model.KhMinName = customerDetail.NameCus;
      this.model.IdKhmin = customerDetail.CusId;
    } else {
      this.model.KhMinName = inputValue;
    }
  }

  // xử lý khi clear hết value, sau đó blur khỏi control thì clear dữ liệu đã gán ngầm trước đó
  customerMinOnBlur() {
    if (this.model.KhMinName === undefined || this.model.KhMinName.length === 0) {
      this.model.IdKhmin = 0;
    }
  }

  // xử lý sau khi chọn khách hàng, gán ngầm Id khách hàng
  customerMaxAutocompleteCallback(value) {
    const inputValue = this.model.KhMinName;
    const customerDetail: CustomerDetail = Object.assign({}, this.lstCustomer.find(x => x.CusId === value));
    if (customerDetail != null && customerDetail.CusId > 0) {
      this.model.KhMaxName = customerDetail.NameCus;
      this.model.IdKhmax = customerDetail.CusId;
    } else {
      this.model.KhMaxName = inputValue;
    }
  }

  // xử lý khi clear hết value, sau đó blur khỏi control thì clear dữ liệu đã gán ngầm trước đó
  customerMaxOnBlur() {
    if (this.model.KhMaxName === undefined || this.model.KhMaxName.length === 0) {
      this.model.IdKhmax = 0;
    }
  }

  clearLoaiTienNorth() {

  }

  changeEffectiveDate(value: Date, row: ProductCIFPrice) {
    const dateNow = new Date();
    row.EffectiveDate.setHours(dateNow.getHours());
    row.EffectiveDate.setMinutes(dateNow.getMinutes());
    row.EffectiveDate.setMilliseconds(dateNow.getMilliseconds());
  }

  changeEffectiveDateNorth(value: Date, row: ProductCIFPrice) {
    const dateNow = new Date();
    row.EffectiveDateNorth.setHours(dateNow.getHours());
    row.EffectiveDateNorth.setMinutes(dateNow.getMinutes());
    row.EffectiveDateNorth.setMilliseconds(dateNow.getMilliseconds());
  }

  checkIsDefaultNorth(index: number) {
    const valueOriginal = this.model.ListProductCIFPrice[index].IsDefaultNorth;
    this.model.ListProductCIFPrice.forEach(item => {
      item.IsDefaultNorth = 0;
    });
    this.model.ListProductCIFPrice[index].IsDefaultNorth = valueOriginal ? 1 : 0;
  }
}
