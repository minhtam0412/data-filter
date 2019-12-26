import {AfterViewChecked, AfterViewInit, Component, OnInit} from '@angular/core';
import {ReportColumnView} from '../reportcolumnview/model/reportcolumnview';
import {ReportmainService} from './reportmain.service';
import {NotificationService} from '../../common/notifyservice/notification.service';
import {ReportTotal} from '../model/reporttotal';
import {ReportcolumnviewService} from '../reportcolumnview/reportcolumnview.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {ReportpreviewComponent} from '../reportpreview/reportpreview.component';
import {QuantitytranformComponent} from '../quantitytranform/quantitytranform.component';
import {UnitpricetranformComponent} from '../unitpricetranform/unitpricetranform.component';
import {ComfirmDialogComponent} from '../../common/comfirm-dialog/comfirm-dialog.component';
import {AngularGridInstance, Column, FieldType, Formatter, Formatters, GridOption, GridService} from 'angular-slickgrid';

@Component({
  selector: 'app-reportmain',
  templateUrl: './reportmain.component.html',
  styleUrls: ['./reportmain.component.css']
})
export class ReportmainComponent implements OnInit, AfterViewInit, AfterViewChecked {

  dropdownListOriginal: ReportColumnView[] = [];
  dropdownListByUser: ReportColumnView[] = [];
  selectedItems: ReportColumnView[] = [];
  dropdownSettings = {};
  lstDataSearch: ReportTotal[] = []; // list chứa dữ liệu sau khi tìm kiếm
  lstDataResult: ReportTotal[] = []; // list chứa dữ liệu sau khi merge
  lstResultFilter: ReportTotal[] = []; // list chứa dữ liệu filter sau khi tìm kiếm
  searchString: string;
  filterString: string;

  angularGrid2: AngularGridInstance;
  columnDefinitions2: Column[];
  gridOptions2: GridOption;
  dataset2: any[];
  gridObj2: any;
  dataView: any;

  bolIsComplete = false;


  constructor(private modalService: NgbModal, private notificationService: NotificationService,
              private reportcolumnviewService: ReportcolumnviewService, private service: ReportmainService) {
    this.dropdownListOriginal = [
      // {ColumnCode: 'STT', ColumnName: 'STT', SortOrder: -1},
      {ColumnCode: 'PhanLoai', ColumnName: 'Phân loại', SortOrder: 0},
      {ColumnCode: 'NamNhap', ColumnName: 'Năm nhập', SortOrder: 1},
      {ColumnCode: 'NgayNhap', ColumnName: 'Ngày đăng ký', SortOrder: 2},
      {ColumnCode: 'ChiCucHaiQuan', ColumnName: 'Chi cục hải quan', SortOrder: 3},
      {ColumnCode: 'CangXuatNhap', ColumnName: 'Cảng xuất nhập', SortOrder: 4},
      {ColumnCode: 'TenLoHang', ColumnName: 'Tên lô hàng', SortOrder: 5},
      {ColumnCode: 'MaDoanhNghiep', ColumnName: 'Mã doanh nghiệp', SortOrder: 6},
      {ColumnCode: 'DonViDoiTac', ColumnName: 'Đơn vị đối tác', SortOrder: 7},
      {ColumnCode: 'HsCode', ColumnName: 'Hs Code', SortOrder: 8},
      {ColumnCode: 'ChungLoaiHangHoaXuatNhap', ColumnName: 'Tên hàng', SortOrder: 9},
      {ColumnCode: 'DoanhNghiepXuatNhap', ColumnName: 'Tên doanh nghiệp XNK', SortOrder: 10},
      {ColumnCode: 'NuocXuatXu', ColumnName: 'Tên nuớc xuất xứ', SortOrder: 11},
      {ColumnCode: 'DVT', ColumnName: 'Đơn vị tính', SortOrder: 12},
      {ColumnCode: 'Luong', ColumnName: 'Số lượng', SortOrder: 13},
      {ColumnCode: 'SoLuongQuyDoi', ColumnName: 'Số lượng quy đổi (kg)', SortOrder: 14},
      {ColumnCode: 'DonGia', ColumnName: 'Đơn giá', SortOrder: 15},
      {ColumnCode: 'DonGiaUSD', ColumnName: 'Đơn giá quy đổi (USD/kg)', SortOrder: 16},
      {ColumnCode: 'TriGia', ColumnName: 'Trị giá', SortOrder: 17},
      {ColumnCode: 'NgoaiTeThanhToan', ColumnName: 'Nguyên tệ', SortOrder: 18},
      {ColumnCode: 'TyGiaVnd', ColumnName: 'Tỷ giá VND', SortOrder: 19},
      {ColumnCode: 'TriGiaVnd', ColumnName: 'Trị giá VND', SortOrder: 20},
      {ColumnCode: 'TyGiaUsd', ColumnName: 'Tỷ giá USD', SortOrder: 21},
      {ColumnCode: 'TriGiaUsd', ColumnName: 'Trị giá USD', SortOrder: 22},
      {ColumnCode: 'DieuKienGiaoHang', ColumnName: 'Điều kiện giao hàng', SortOrder: 23},
      {ColumnCode: 'DieuKienThanhToan', ColumnName: 'Điều kiện thanh toán', SortOrder: 24},
      {ColumnCode: 'Tsxnk', ColumnName: 'TS_XNK', SortOrder: 25},
      {ColumnCode: 'ThueXnk', ColumnName: 'Thuế XNK', SortOrder: 26},
      {ColumnCode: 'Tsttdb', ColumnName: 'TS_TTDB', SortOrder: 27},
      {ColumnCode: 'ThueTtdb', ColumnName: 'Thuế TTDB', SortOrder: 28},
      {ColumnCode: 'Tsvat', ColumnName: 'TS_VAT', SortOrder: 29},
      {ColumnCode: 'ThueVat', ColumnName: 'Thuế VAT', SortOrder: 30},
      {ColumnCode: 'PhuThu', ColumnName: 'Phụ thu', SortOrder: 31},
      {ColumnCode: 'MienThue', ColumnName: 'Miễn thuế', SortOrder: 32},
      {ColumnCode: 'PhuongTienVanTai', ColumnName: 'Phương tiện vận tải', SortOrder: 33},
      {ColumnCode: 'TenPhuongTienVanTai', ColumnName: 'Tên phương tiện vận tải', SortOrder: 34},
      {ColumnCode: 'NuocXuatKhau', ColumnName: 'Nước xuất khẩu', SortOrder: 35},
      {ColumnCode: 'NuocNhapKhau', ColumnName: 'Nước nhập khẩu', SortOrder: 36},
      {ColumnCode: 'CangNuocNgoai', ColumnName: 'Cảng nước ngoài', SortOrder: 37},
      {ColumnCode: 'PhanLoaiTrangThai', ColumnName: 'Phân loại trạng thái', SortOrder: 38},
      {ColumnCode: 'SoToKhai', ColumnName: 'Số tờ khai', SortOrder: 39},
    ];
    this.loadData('1');
  }

  angularGridReady2(angularGrid: AngularGridInstance) {
    this.angularGrid2 = angularGrid;
    this.gridObj2 = angularGrid && angularGrid.slickGrid || {};
    this.dataView = angularGrid.dataView;
  }

  prepareGrid() {
    this.columnDefinitions2 = [];
    this.gridOptions2 = {
      enableAutoResize: false,
      enableCellNavigation: true,
      enableFiltering: true,
      checkboxSelector: {
        // you can toggle these 2 properties to show the "select all" checkbox in different location
        hideInFilterHeaderRow: false,
        hideInColumnTitleRow: true
      },
      rowSelectionOptions: {
        // True (Single Selection), False (Multiple Selections)
        selectActiveRow: false
      },
      enableCheckboxSelector: true,
      enableRowSelection: true,
    };
  }

  handleSelectedRowsChanged2(e, args) {
    console.log('args.rows', args.rows);
    if (Array.isArray(args.rows)) {
      // this.selectedTitles = args.rows.map(idx => {
      //   const item = this.gridObj2.getDataItem(idx);
      //   return item.title || '';
      // });
    }
  }


  ngOnInit() {
    this.selectedItems = [];
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'ColumnCode',
      textField: 'ColumnName',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 4,
      allowSearchFilter: false
    };
    this.prepareGrid();
  }

  loadData(viewType: string) {
    this.reportcolumnviewService.getData(viewType).subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.notificationService.showError(res.MessageText);
            this.dropdownListByUser = [];
          } else {
            this.dropdownListByUser = res.RepData;
            this.selectedItems = this.dropdownListByUser;

            if (this.dropdownListByUser.length === 0) {
              this.dropdownListByUser = this.dropdownListOriginal;
              this.selectedItems = this.dropdownListByUser;
              this.saveData(this.selectedItems);
            }
            this.selectedItems.forEach(value => {
              const itemOriginal = this.dropdownListOriginal.find(originalItem => {
                return originalItem.ColumnCode === value.ColumnCode;
              });
              if (itemOriginal != null) {
                value.SortOrder = itemOriginal.SortOrder;
              }
            });
          }
        },
        error: (err) => {
          console.log(err);
          this.notificationService.showError('Lỗi tìm kiếm thông tin!');
        },
        complete: () => {
          this.bolIsComplete = true;
          console.log('done');
        }
      }
    );
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

  ngAfterViewInit(): void {

  }

  ngAfterViewChecked(): void {
    if (this.bolIsComplete) {
      // this.prepareGrid();
    }
  }


  updateHighlightFilter(previousItemMetadata: any, bolIsResetStyle: boolean = false) {
    const newCssClass = 'highlight-filter';

    return (rowNumber: number) => {
      const item = this.dataView.getItem(rowNumber) as ReportTotal;
      let meta = {
        cssClasses: ''
      };
      if (typeof previousItemMetadata === 'object') {
        meta = previousItemMetadata(rowNumber);
      }
      if (meta && item && item.id) {
        if (item.IsFilter === 1) {
          if (bolIsResetStyle) {
            meta.cssClasses.replace('highlight-filter', '');
          } else {
            meta.cssClasses = (meta.cssClasses || '') + ' ' + newCssClass;
          }

        }
      }

      return meta;
    };
  }

  genSelectedColumns() {
    this.selectedItems.forEach(value => {
      const originalColumn = this.dropdownListOriginal.find(value1 => {
        return value1.ColumnCode === value.ColumnCode;
      });
      value.SortOrder = originalColumn.SortOrder;
    });
    const lstColumns = this.selectedItems.sort((a, b) => {
      if (a.SortOrder > b.SortOrder) {
        return 1;
      } else {
        if (a.SortOrder < b.SortOrder) {
          return -1;
        } else {
          return 0;
        }
      }
    });
    this.columnDefinitions2.length = 0;
    if (lstColumns.length > 0) {
      lstColumns.forEach(value => {
        this.columnDefinitions2.push({
          id: value.ColumnCode, name: value.ColumnName, field: value.ColumnCode,
          sortable: true,
          minWidth: value.ColumnName.length + 180,
        });
      });
    }
    this.angularGrid2.extensionService.renderColumnHeaders(this.columnDefinitions2);
    this.angularGrid2.extensionService.autoResizeColumns();
  }

  onChanged() {
    this.genSelectedColumns();
    this.saveData(this.selectedItems);
  }

  checkSelected(colCode: string) {
    const index = this.selectedItems.findIndex(value => {
      return value.ColumnCode.toLowerCase() === colCode.toLowerCase();
    });
    return index > -1;
  }

  confirmOverWrite() {
    if (this.lstDataResult.length > 0) {
      const modalRef = this.modalService.open(ComfirmDialogComponent, {
        backdrop: false, scrollable: true, centered: true
      });

      modalRef.componentInstance.contentMessage = 'Bạn có chắc chắn muốn overwrite dữ liệu hiện tại?';
      modalRef.result.then((result) => {
        if (result !== undefined && result === true) {
          this.overWrite();
        }
      });
    } else {
      this.overWrite();
    }
  }

  searchProduct() {
    if (this.searchString === undefined || this.searchString.trim().length === 0) {
      this.notificationService.showInfo('Vui lòng nhập chủng loại hàng hoá cần tìm!');
      return;
    }
    this.service.getData(this.searchString.trim()).subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.notificationService.showError(res.MessageText);
            this.lstDataSearch = [];
          } else {
            this.lstDataSearch = res.RepData;
            this.lstDataSearch.every(_ => _.Selected = true);
            // fill the dataset with your data
            this.dataset2 = this.lstDataSearch;
            this.angularGrid2.gridService.resetGrid();
          }
        },
        error: (err) => {
          console.log(err);
          this.notificationService.showError('Lỗi tìm kiếm thông tin!');
        },
        complete: () => {

        }
      }
    );
  }

  isAllChecked() {
    return this.lstDataSearch.every(_ => _.Selected);
  }

  checkAll(ev) {
    this.lstDataSearch.forEach(x => x.Selected = ev.target.checked);
  }

  overWrite() {
    if (this.lstDataSearch.length === 0) {
      this.notificationService.showInfo('Không có dữ liệu để overwrite!');
      return;
    }

    // kiểm tra có dữ liệu được chọn để overwite
    const checkSelected = this.lstDataSearch.every(value => {
      return value.Selected === false;
    });

    if (checkSelected) {
      this.notificationService.showInfo('Vui lòng chọn dữ liệu để overwrite!');
      return;
    }

    this.lstDataResult.length = 0;
    this.lstDataSearch.forEach(value => {
      if (value.Selected) {
        this.lstDataResult.push(Object.assign({}, value));
      }
    });
    this.notificationService.showInfo('Overwrite thành công!');
  }

  merge() {
    if (this.lstDataSearch.length === 0) {
      this.notificationService.showInfo('Không có dữ liệu để merge!');
      return;
    }

    this.lstDataSearch.forEach(value => {
      // kiểm tra các phần tử tìm kiếm được đã tồn tại trong danh sách result hay chưa
      const index = this.lstDataResult.findIndex(rsl => {
        return rsl.id === value.id;
      });
      // nếu chưa tồn tại thì thêm vào danh sách result
      if (index < 0 && value.Selected) {
        this.lstDataResult.push(Object.assign({}, value));
      }
    });
    this.notificationService.showInfo('Merge dữ liệu thành công!');
  }

  previewResult() {
    if (this.lstDataResult.length === 0) {
      this.notificationService.showInfo('Không có dữ liệu để Preview!');
      return;
    }
    const modalRef = this.modalService.open(ReportpreviewComponent, {
      backdrop: 'static', scrollable: true, centered: true, backdropClass: 'backdrop-modal', size: 'xl'
    });

    // modalRef.componentInstance.lstDataResult = Object.assign([], this.lstDataResult);
    // modalRef.componentInstance.dropdownListOriginal = this.dropdownListOriginal;
    // modalRef.componentInstance.selectedItems = this.selectedItems;
    // modalRef.componentInstance.dropdownSettings = this.dropdownSettings;

    // xử lý sau khi đóng dialog, thực hiện load lại dữ liệu nếu muốn
    modalRef.result.then((result) => {
      if (result !== undefined) {
        // update lại giao diện grid nếu người dùng có thay trong popup
        this.selectedItems = result;
      }
    }, (reason) => {
    });
  }

  quantityTranform() {
    const modalRef = this.modalService.open(QuantitytranformComponent, {
      backdrop: 'static', scrollable: true, centered: true, backdropClass: 'backdrop-modal', size: 'xl'
    });

    modalRef.componentInstance.lstDataResult = Object.assign([], this.lstDataResult);

    // xử lý sau khi đóng dialog
    modalRef.result.then((result) => {
      if (result !== undefined) {
        this.lstDataResult = result;
      }
    }, (reason) => {
    });
  }

  unitPriceTranform() {
    if (this.lstDataResult.length === 0) {
      this.notificationService.showInfo('Không có dữ liệu!');
      return;
    }

    const modalRef = this.modalService.open(UnitpricetranformComponent, {
      backdrop: 'static', scrollable: true, centered: true, backdropClass: 'backdrop-modal', size: 'xl'
    });

    modalRef.componentInstance.lstDataResult = Object.assign([], this.lstDataResult);

    // xử lý sau khi đóng dialog
    modalRef.result.then((result) => {
      if (result !== undefined) {
        this.lstDataResult = result;
        this.notificationService.showInfo('Đã cập nhật đơn giá!');
      }
    }, (reason) => {
    });
  }

  previewFilter() {
    if (this.filterString === undefined || this.filterString.trim().length === 0) {
      this.notificationService.showInfo('Vui lòng nhập từ khoá cần filter!');
      return;
    }
    this.filterByString();
  }

  selectFilter() {
    if (this.lstDataSearch.length === 0) {
      this.notificationService.showInfo('Không có dữ liệu để filter!');
      return;
    }

    if (this.lstResultFilter.length === 0) {
      this.notificationService.showInfo('Không có dữ liệu để select!');
      return;
    }

    this.lstDataSearch = Object.assign([], this.lstResultFilter);
    this.lstDataSearch.forEach(value => {
      value.Selected = true;
      value.IsFilter = 0;
    });

    this.dataset2 = this.lstDataSearch;
  }

  ConvertUnsign(source, isTransSpecialChar = true) {
    let str = source;
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, 'a');
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, 'e');
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, 'i');
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, 'o');
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, 'u');
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, 'y');
    str = str.replace(/đ/g, 'd');

    if (isTransSpecialChar) {
      str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, ' ');
    }
    str = str.replace(/ + /g, ' ');
    str = str.trim();
    return str;
  }

  filterByString() {
    let arrResult: ReportTotal[] = [];
    this.lstResultFilter.length = 0;
    const filterString = this.ConvertUnsign(this.filterString.toLowerCase().trim(), false);
    arrResult = this.lstDataSearch.filter(value => {
      return value.SearchString.includes(filterString) || value.ChungLoaiHangHoaXuatNhap.includes(filterString) ||
        value.SearchDoanhNghiepXuatNhap.includes(filterString) || value.DoanhNghiepXuatNhap.includes(filterString) ||
        value.SearchDonViDoiTac.includes(filterString) || value.DonViDoiTac.includes(filterString) ||
        value.SearchNuocXuatXu.includes(filterString) || value.NuocXuatXu.includes(filterString);
    });
    if (arrResult.length === 0) {
      this.notificationService.showInfo('Không có dữ liệu!');
      return;
    }
    this.lstDataSearch.forEach(value => {
      value.IsFilter = 0;
    });
    arrResult.forEach(value => {
      value.IsFilter = 1;
    });
    arrResult.forEach(value => {
      this.lstResultFilter.push(Object.assign({}, value));
    });

    this.dataView.getItemMetadata = this.updateHighlightFilter(this.dataView.getItemMetadata);
    // also re-render the grid for the styling to be applied right away
    this.gridObj2.invalidate();
    this.gridObj2.render();
  }

  deleteFilter() {
    if (this.lstDataSearch.length === 0) {
      this.notificationService.showInfo('Không có dữ liệu!');
      return;
    }

    this.lstResultFilter.forEach(valueFiltered => {
      const index = this.lstDataSearch.findIndex(valueOriginal => {
        return valueFiltered.id === valueOriginal.id;
      });
      if (index > -1) {
        this.lstDataSearch.splice(index, 1);
      }
    });
    console.log(this.lstDataSearch);
    this.dataset2 = this.lstDataSearch;
    this.angularGrid2.gridService.resetGrid();
  }
}
