import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Key_DefaultPageLimit, Key_MaxRow, Key_Delete_Icon, Key_Duplicate_Icon } from 'src/app/common/config/globalconfig';
import { PaginationComponent } from 'src/app/pagination/pagination.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { LocalNumberPipe } from 'src/app/common/locale/pipes/local-number-pipe.service';
import { ComfirmDialogComponent } from 'src/app/common/comfirm-dialog/comfirm-dialog.component';
import { Globalfunctions } from 'src/app/common/globalfunctions/globalfunctions';
import { CustomerDeliveryAddress } from 'src/app/lists/customerdetail/model/customerdeliveryaddress';
import { CustomerDetail } from 'src/app/lists/customerdetail/model/customerdetail';
import { ProductDetail } from 'src/app/lists/product/model/productdetail';
import { PricecalculatorService } from '../pricecalculator.service';
import { ProductService } from 'src/app/lists/product/product.service';
import { ProductCIFPrice } from 'src/app/lists/product/model/productcifprice';
import { PriceSpreadsheets } from '../model/pricespreadsheets';
import { AggregatecostsService } from 'src/app/lists/aggregatecosts/aggregatecosts.service';
import { AggregateCosts } from 'src/app/lists/aggregatecosts/model/aggregatecosts';
import { ShippedpriceService } from 'src/app/lists/shippedprice/shippedprice.service';
import { ShippedPrice } from 'src/app/lists/shippedprice/model/shippedprice';
import { AmountDefine } from '../model/amountdefine';
import { localeData } from 'moment';
import { formatDate } from '@angular/common';
import { SessionService } from 'src/app/common/locale/session-service.service';
import { Sales } from 'src/app/lists/customerdetail/model/Sales';
import { ActivatedRoute, Router } from '@angular/router';

declare var jquery: any;
declare var $: any;
@Component({
  selector: 'app-pricecalculator',
  templateUrl: './pricecalculator.component.html',
  styleUrls: ['./pricecalculator.component.css']
})
export class PricecalculatorComponent implements OnInit {

  objModel: PriceSpreadsheets;
  objModelBackup: PriceSpreadsheets;
  isNewModel: boolean;
  selectIndex: number;
  objPriced: ProductDetail;

  lstDataResult: PriceSpreadsheets[] = [];
  lstProduct: ProductDetail[] = [];
  lstCustomer: CustomerDetail[] = [];
  lstAreaList: CustomerDeliveryAddress[] = [];
  lstAggregateCosts: AggregateCosts[] = [];
  lstShippedprice: ShippedPrice[] = [];
  lstAmountDefine: AmountDefine[] = [];
  lstProductCIFPrice: ProductCIFPrice[] = [];

  storeid: number;
  total = 10;
  page = 1;
  limit = Key_DefaultPageLimit;
  deleteIcon = Key_Delete_Icon;
  duplicateIcon = Key_Duplicate_Icon;
  productId_Select: number = null;
  customerId_Select: number = null;

  static session: SessionService = new SessionService();

  @ViewChild(PaginationComponent, { static: true }) pagingComponent: PaginationComponent;
  @ViewChild('closeAddExpenseModal', { static: true }) closeAddExpenseModal: ElementRef;

  constructor(private pricecalculatorService: PricecalculatorService, private modalService: NgbModal, private toastr: ToastrService,
    private localNumberPipe: LocalNumberPipe, private productService: ProductService, private aggregatecostsService: AggregatecostsService,
    private shippedpriceService: ShippedpriceService, private activatedRoute: ActivatedRoute,
    private router: Router) {
    this.isNewModel = true;
    this.objModel = new PriceSpreadsheets();
    this.objModelBackup = new PriceSpreadsheets();
    this.objPriced = new ProductDetail();
  }

  ngOnInit() {
    this.storeid = this.activatedRoute.snapshot.params['storeid'];
    console.log(this.storeid);

    this.GetProductCache();
    this.GetCustomerCache();
    this.GetAggregatecosts();
    this.GetShippedprice();
    this.GetAmountDefine();

  }

  ngAfterViewInit(): void {
    this.LoadData();
    $(function () {
      $('[data-toggle="tooltip"]').tooltip()
    })
  }

  //--------------------tính giá vốn tại kho--------------------
  reportDataResultProduct() {
    this.objModel.CpluuKhoTotal = this.objModel.CpluuKho * (this.objModel.TgluuKhoTb / 30);

    if (this.objModel.MaHh != "" && this.objModel.MaHh != null && this.objModel.MaHh != undefined &&
      this.objModel.GiaCif != null && this.objModel.GiaCif != undefined && this.objModel.GiaCif.toString() != "") {
      var giamua = Number(this.objModel.GiaCif.toString()) * this.objModel.TyGia;
      var thuenhapkhau = giamua * (this.objModel.ThueNk / 100);
      var phithuekho = (this.objModel.TgluuKhoTb == 0 || this.objModel.TgluuKhoTb == null || this.objModel.TgluuKhoTb == undefined) ? 0 : this.objModel.CpluuKho * (this.objModel.TgluuKhoTb / 30);
      var chenhlechtygia = giamua * (this.lstAggregateCosts.find(x => x.LoaiCp == "PhiChenhLechTyGia").ChiPhi / 100);
      this.objModel.GiaVonTaiKho = Math.round(giamua + thuenhapkhau + chenhlechtygia + Number(this.objModel.CpveKho) + phithuekho + Number(this.objModel.CpbocXep));
    }
    else {
      this.objModel.GiaVonTaiKho = 0;
    }

    if (this.objModel.GiaVonGiaoToiKh != 0)
      this.reportDataResultCustomer();

    if (this.objModel.LoiNhuanDuKien != 0)
      this.reportDataResultExpectedProfit();
  }

  //--------------------tính giá vốn giao đến khách hàng--------------------
  reportDataResultCustomer() {
    if (this.objModel.MaKh != "" && this.objModel.MaKh != null && this.objModel.MaKh != undefined &&
      this.objModel.MaKhuVuc != null && this.objModel.MaKhuVuc != undefined && this.objModel.MaKhuVuc != "") {
      this.GetAmounCost(this.objModel);
      this.objModel.GiaVonGiaoToiKh = this.objModel.GiaVonTaiKho + this.objModel.Cpgiao;
      //backup
      this.objModelBackup.Cpgiao = this.objModel.Cpgiao;

      this.reportDataResultExpectedProfit();
    }
    else {
      this.objModel.GiaVonGiaoToiKh = 0;
    }
  }

  reportDataResultCustomer__Change() {
    this.objModel.GiaVonGiaoToiKh = this.objModel.GiaVonTaiKho + Number(this.objModel.Cpgiao.toString());

    if (this.objModel.LoiNhuanDuKien != 0)
      this.reportDataResultExpectedProfit();
  }

  //--------------------tính lợi nhuận dự kiến--------------------
  reportDataResultExpectedProfit() {
    if (this.objModel.GiaBanDuKien != 0 && this.objModel.MaKh != "" && this.objModel.MaKh != null && this.objModel.MaKh != undefined) {
      //Chi phí tài chính (công nợ) = % lãi suất TB * Giá bán (trên 30 ngày)
      if (this.objModel.HanThanhToan != 0 && this.objModel.HanThanhToan != null && this.objModel.HanThanhToan != undefined) {
        this.objModel.CptaiChinhTotal = ((this.objModel.CptaiChinh / 100) * this.objModel.GiaBanDuKien) * (this.objModel.HanThanhToan / 30);
      } else {
        this.objModel.CptaiChinhTotal = 0;
      }

      this.objModel.CpquanLyTtTotal = (this.objModel.CpquanLyTt / 100) * (this.objModel.GiaBanDuKien - this.objModel.GiaVonGiaoToiKh - Number(this.objModel.CpquanLyGtTotal.toString()) - this.objModel.CptaiChinhTotal);
      var chiphikhac = this.objModel.CpquanLyTtTotal + Number(this.objModel.CpquanLyGtTotal.toString()) + this.objModel.CptaiChinhTotal;

      //[(Giá bán dự kiến - (Giá vốn giao tới Khách hàng + Chi phí khác)) * 100] / Giá bán dự kiến
      this.objModel.LoiNhuanDuKien = ((this.objModel.GiaBanDuKien - this.objModel.GiaVonGiaoToiKh - chiphikhac) * 100) / this.objModel.GiaBanDuKien;
      this.objModel.LoiNhuanDuKien = Math.round(this.objModel.LoiNhuanDuKien * 1000 + Number.EPSILON) / 1000;

    }
    else {
      this.objModel.LoiNhuanDuKien = 0;
      this.objModel.CptaiChinhTotal = 0;
      this.objModel.CpquanLyTtTotal = 0;
      // this.objModel.CpquanLyGtTotal = 0;
    }
  }

  reportSalePriceExpected() {
    if (this.objModel.MaKh != "" && this.objModel.MaKh != null && this.objModel.MaKh != undefined && this.objModel.MaHh != "" && this.objModel.MaHh != null && this.objModel.MaHh != undefined) {
      var tem_hanthanhtoan = 0;
      if (this.objModel.HanThanhToan != 0 && this.objModel.HanThanhToan != null && this.objModel.HanThanhToan != undefined) {
        tem_hanthanhtoan = this.objModel.HanThanhToan / 30;
      }
      var tem_phitt = this.objModel.CpquanLyTt / 100;
      var tem_phigt = this.objModel.CpquanLyGt / 100;
      var tem_phitc = this.objModel.CptaiChinh / 100;

      this.objModel.GiaBanDuKien = (this.objModel.GiaVonGiaoToiKh * (1 - tem_phitt) * 100) / ((100 - Number(this.objModel.LoiNhuanDuKien.toString())) - 100 * (tem_phitt * (1 - tem_phigt - tem_phitc * tem_hanthanhtoan) + tem_phigt + tem_phitc * tem_hanthanhtoan));
      this.objModel.GiaBanDuKien = Math.round(this.objModel.GiaBanDuKien);

      if (this.objModel.HanThanhToan != 0 && this.objModel.HanThanhToan != null && this.objModel.HanThanhToan != undefined) {
        this.objModel.CptaiChinhTotal = ((this.objModel.CptaiChinh / 100) * this.objModel.GiaBanDuKien) * (this.objModel.HanThanhToan / 30);
      } else {
        this.objModel.CptaiChinhTotal = 0;
      }
      this.objModel.CpquanLyGtTotal = Math.round((this.objModel.CpquanLyGt / 100) * this.objModel.GiaBanDuKien);
      this.objModel.CpquanLyTtTotal = (this.objModel.CpquanLyTt / 100) * (this.objModel.GiaBanDuKien - this.objModel.GiaVonGiaoToiKh - Number(this.objModel.CpquanLyGtTotal.toString()) - this.objModel.CptaiChinhTotal);
    }
  }

  //--------------------Load data combobox--------------------
  GetProductCache() {
    this.pricecalculatorService.GetProductCache().subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.lstProduct = res.RepData;
          }
        },
        error: (err) => {
          console.log(err);
        }
      }
    );
  }

  GetCustomerCache() {
    this.pricecalculatorService.GetCustomerCache().subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.lstCustomer = res.RepData;
          }
        },
        error: (err) => {
          console.log(err);
        },
      }
    );
  }

  GetCustomerDeliveryAddress(CusId: number) {
    this.pricecalculatorService.GetCustomerDeliveryAddress(CusId).subscribe(
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
        },
        complete: () => {
          var itemDefault: CustomerDeliveryAddress = this.lstAreaList.find(x => x.IsDefault);
          if (itemDefault != null && itemDefault != undefined) {
            this.objModel.DiaChiGh = itemDefault.DeliveryAdr;
            this.objModel.KhuVucGh = itemDefault.TenKhuVuc;
            this.objModel.MaKhuVuc = itemDefault.MaKhuVuc;
            this.reportDataResultCustomer();
            //backup
            this.objModelBackup.Cpgiao = this.objModel.Cpgiao;
          }
          else {
            this.objModel.GiaVonGiaoToiKh = 0;
          }
        }
      }
    );
  }

  GetShippedprice() {
    this.shippedpriceService.getData("", 0, Key_MaxRow).subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.lstShippedprice = res.RepData;
          }
        },
        error: (err) => {
          console.log(err);
        }
      }
    );
  }

  GetAmountDefine() {
    this.pricecalculatorService.GetAmountDefine().subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.lstAmountDefine = res.RepData;
          }
        },
        error: (err) => {
          console.log(err);
        }
      }
    );
  }

  GetAggregatecosts() {
    this.aggregatecostsService.getData("", 0, Key_MaxRow).subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.lstAggregateCosts = res.RepData;
          }
        },
        error: (err) => {
          console.log(err);
        },
        complete: () => {
          this.loadAggregateCostsDefault();
        }
      }
    );
  }

  loadAggregateCostsDefault() {
    if (this.lstAggregateCosts.length > 0) {
      this.objModel.CpquanLyTt = this.lstAggregateCosts.find(x => x.LoaiCp == "PhiQuanLyTrucTiep").ChiPhi;
      this.objModel.CpquanLyGt = this.lstAggregateCosts.find(x => x.LoaiCp == "PhiQuanLyGianTiep").ChiPhi;
      this.objModel.CptaiChinh = this.lstAggregateCosts.find(x => x.LoaiCp == "LaiSuatTB").ChiPhi;
      this.objModel.ChenhLechTyGia = this.lstAggregateCosts.find(x => x.LoaiCp == "PhiChenhLechTyGia").ChiPhi;
    }
  }
  //--------------------Event Product--------------------
  public Product_autocompleteHeaderTemplate = `
  <div class="header-row">
    <div class="col-3">Mã hàng hóa</div>
    <div class="col-3">Tên hàng hóa</div>
    <div class="col-5">Tên hàng hóa đầy đủ</div>
  </div>`;

  public Product_renderDataRowAutoComplete(data: ProductDetail): string {
    const html = `
    <div class="data-row">
      
      <div class="col-3 text-left">${data.ProductCode == null ? "" : data.ProductCode}</div>
      <div class="col-3 text-left">${data.ProductName == null ? "" : data.ProductName}</div>
      <div class="col-5">${data.ProductNameFull == null ? "" : data.ProductNameFull}</div>
    </div>`;
    return html;
  }

  Product_autocompleteCallback(event) {
    if (this.lstProduct.filter(x => x.ProductCode == this.objModel.MaHh).length <= 0) {
      this.clearCIF();
    }
    var item: ProductDetail = Object.assign({}, this.lstProduct.find(x => x.Id == event));
    if (item.Id != null && item.Id != undefined) {

      this.productId_Select = item.ProductId;
      this.objModel.MaHh = item.ProductCode;
      this.objModel.TenHh = item.ProductName;
      this.objModel.ThueNk = item.ImportTax;
      this.objModel.CpveKho = this.storeid == 1 ? item.CpveKhoLh : item.CpveKhoBn;
      this.objModel.TgluuKhoTb = item.TgTonKhoTb;
      this.objModel.CpbocXep = this.lstAggregateCosts.find(x => x.LoaiCp == "PhiBocXep").ChiPhi;
      this.objModel.CpluuKho = this.lstAggregateCosts.find(x => x.LoaiCp == "PhiLuuKho").ChiPhi;
      this.objModel.ChenhLechTyGia = this.lstAggregateCosts.find(x => x.LoaiCp == "PhiChenhLechTyGia").ChiPhi;
      this.objModel.CpluuKhoTotal = this.objModel.CpluuKho * (this.objModel.TgluuKhoTb / 30);

      this.objModel.IdColorCpveKho = 0;
      this.objModel.IdColorTgluuKhoTb = 0;
      this.objModel.IdColorCpbocXep = 0;
      this.objModel.IdColorCpluuKho = 0;

      //---------Giá bán cũ------------
      this.objPriced.GiaMin = item.GiaMin == null || item.GiaMin == undefined ? 0 : item.GiaMin;
      this.objPriced.KhMinName = item.KhMinName;
      this.objPriced.GiaMax = item.GiaMax == null || item.GiaMax == undefined ? 0 : item.GiaMax;
      this.objPriced.KhMaxName = item.KhMaxName;
      this.objPriced.GiaTb = item.GiaTb == null || item.GiaTb == undefined ? 0 : item.GiaTb;

      //---------Default giá CIF-------
      this.lstProductCIFPrice = item.ListProductCIFPrice;
      if (this.storeid == 2) {
        this.lstProductCIFPrice = this.lstProductCIFPrice.filter(x => x.Cifnorth != null && x.LoaiTienNorth != null && x.LoaiTienNorth != "");
      }
      var itemDefault: ProductCIFPrice = this.storeid == 1 ? this.lstProductCIFPrice.find(x => x.IsDefault == 1) : this.lstProductCIFPrice.find(x => x.IsDefaultNorth == 1);
      if (itemDefault != null && itemDefault != undefined) {
        this.objModel.GiaCif = this.storeid == 1 ? itemDefault.Cifsouth : itemDefault.Cifnorth;
        this.objModel.LoaiTien = this.storeid == 1 ? itemDefault.LoaiTien : itemDefault.LoaiTienNorth;
        this.objModel.TyGia = this.storeid == 1 ? itemDefault.TyGia : itemDefault.TyGiaNorth;

        this.objModel.IdColorGiaCif = 0;
        this.objModel.IdColorTyGia = 0;

        //------Reload giá bán hiện hành--------
        if (this.objModel.MaKh != undefined && this.objModel.MaHh != null && this.objModel.MaKh != "" && this.customerId_Select != null) {
          var tem_customter: CustomerDetail = Object.assign({}, this.lstCustomer.find(x => x.CusId == this.customerId_Select));
          if (tem_customter.CusId != null && tem_customter.CusId != undefined && this.productId_Select != null) {
            this.GetPriceNow(tem_customter);
          }
        }
        this.reportDataResultProduct();
      }
      else {
        this.objModel.GiaVonTaiKho = 0;
      }

    }
  }

  Product_onKeydown(event) {
    if (event.key === 'Enter') {
      //----
    }
  }

  clearCIF() {
    this.lstProductCIFPrice = [];
    this.objModel.GiaCif = 0;
    this.objModel.LoaiTien = "";
    this.objModel.TyGia = 0;
  }

  clearValueProduct() {
    if (this.objModel.MaHh == null || this.objModel.MaHh == undefined || this.objModel.MaHh == "") {
      this.clearCIF();
      this.objModel.TenHh = "";
      this.objModel.ThueNk = 0;
      this.objModel.CpveKho = 0;
      this.objModel.TgluuKhoTb = 0;
      this.objModel.CpbocXep = 0;
      this.objModel.CpluuKho = 0;
      this.objModel.GiaVonTaiKho = 0;
      this.objPriced = new ProductDetail();
      this.productId_Select = null;
    }
  }
  //--------------------Event CIF---------------------
  public CIF_autocompleteHeaderTemplate = `
  <div class="header-row">
    <div class="col-4">Ngày tờ khai</div>
    <div class="col-3">Giá CIF</div>
    <div class="col-3">Loại tiền</div>
  </div>`;

  public CIF_renderDataRowAutoComplete(data: ProductCIFPrice): string {
    const html = `
    <div class="data-row">
      <div class="col-4 text-left">${data.EffectiveDate == null ? "" : formatDate(data.EffectiveDate, "dd/MM/yyyy", PricecalculatorComponent.session.locale)}</div>
      <div class="col-3 text-left">${data.Cifsouth == null ? "" : data.Cifsouth}</div>
      <div class="col-3">${data.LoaiTien == null ? "" : data.LoaiTien}</div>
    </div>`;
    return html;
  }

  public CIF_renderDataRowAutoComplete_North(data: ProductCIFPrice): string {
    const html = `
    <div class="data-row">
      <div class="col-4 text-left">${data.EffectiveDate == null ? "" : formatDate(data.EffectiveDateNorth, "dd/MM/yyyy", PricecalculatorComponent.session.locale)}</div>
      <div class="col-3 text-left">${data.Cifsouth == null ? "" : data.Cifnorth}</div>
      <div class="col-3">${data.LoaiTien == null ? "" : data.LoaiTienNorth}</div>
    </div>`;
    return html;
  }

  CIF_autocompleteCallback(event) {
    var item: ProductCIFPrice = Object.assign({}, this.lstProductCIFPrice.find(x => x.Id == event));
    if (item.Id != null && item.Id != undefined) {
      this.objModel.GiaCif = this.storeid == 1 ? item.Cifsouth : item.Cifnorth;
      this.objModel.LoaiTien = this.storeid == 1 ? item.LoaiTien : item.LoaiTienNorth;
      this.objModel.TyGia = this.storeid == 1 ? item.TyGia : item.TyGiaNorth;;
      this.objModel.IdColorGiaCif = 0;
      this.objModel.IdColorTyGia = 0;
      this.reportDataResultProduct();
    }
  }

  CIF_onKeydown(event) {
    if (event.key === 'Enter') {
      //----
    }
  }

  clearValueCIF() {
    if (this.objModel.GiaCif == undefined || this.objModel.GiaCif == null || this.objModel.GiaCif.toString() == "") {
      this.objModel.LoaiTien = "";
      this.objModel.TyGia = 0;
    }

  }
  //--------------------Event Customer---------------------
  public Customer_autocompleteHeaderTemplate = `
    <div class="header-row">
      <div class="col-3">Mã khách hàng</div>
      <div class="col-3">Tên khách hàng</div>
      <div class="col-5">Tên khách hàng đầy đủ</div>
    </div>`;

  public Customer_renderDataRowAutoComplete(data: CustomerDetail): string {
    const html = `
      <div class="data-row">
        
        <div class="col-3 text-left">${data.CustomerCode == null ? "" : data.CustomerCode}</div>
        <div class="col-3 text-left">${data.NameCus == null ? "" : data.NameCus}</div>
        <div class="col-5">${data.FullnameCus == null ? "" : data.FullnameCus}</div>
      </div>`;
    return html;
  }

  Customer_autocompleteCallback(event) {
    if (this.objModel.MaHh == undefined || this.objModel.MaHh == null || this.objModel.MaHh == "") {
      this.toastr.warning('Hãy chọn hàng hóa', 'Thông báo!');
      return;
    }
    if (this.lstCustomer.filter(x => x.CustomerCode == this.objModel.MaKh).length <= 0) {
      this.clearAreaList();
    }
    var item: CustomerDetail = Object.assign({}, this.lstCustomer.find(x => x.CusId == event));
    if (item.CusId != null && item.CusId != undefined && this.productId_Select != null) {
      this.customerId_Select = item.CusId;
      this.objModel.MaKh = item.CustomerCode;
      this.objModel.TenKh = item.NameCus;

      this.GetPriceNow(item);
      this.GetCustomerDeliveryAddress(item.CusId);
    }
  }

  GetPriceNow(item: CustomerDetail) {
    //-get Giá bán hiện hành
    var tem_GiaBanHienHanh = 0;
    //Kiểm tra dữ liệu có nhiều hàng hóa bán
    var item_sales: Sales = null;
    if (item.lstSales.filter(x => x.ProductId == this.productId_Select).length > 1)
      item_sales = item.lstSales.filter(x => x.ProductId == this.productId_Select)[0];
    else
      item_sales = item.lstSales.find(x => x.ProductId == this.productId_Select);
    //----------------------
    if (item_sales != null) {
      if (item_sales.PayType == 0)
        tem_GiaBanHienHanh = item_sales.GiaDong;
      else
        tem_GiaBanHienHanh = item_sales.GiaNgoaiTe * this.lstAggregateCosts.find(x => x.LoaiCp == item_sales.LoaiNgoaiTe).ChiPhi;

      //tìm hạn thanh toán trong lịch sử giá
      this.objModel.HanThanhToan = item_sales.HanTt;
    }
    else {
      this.objModel.HanThanhToan = item.PaymentExpired;
    }

    this.objModel.GiaBanHienHanh = this.objModel.GiaBanDuKien = Math.round(tem_GiaBanHienHanh);
    this.objModel.CpquanLyGtTotal = (this.objModel.CpquanLyGt / 100) * this.objModel.GiaBanDuKien;

    this.objModel.IdColorGiaBanDuKien = 0;
    this.objModel.IdColorCpquanLyGtTotal = 0;
  }

  Customer_onKeydown(event) {
    if (event.key === 'Enter') {
      //----
    }
  }

  clearValueCustomer() {
    if (this.objModel.MaKh == "" || this.objModel.MaKh == null || this.objModel.MaKh == undefined) {
      this.clearAreaList();
      this.objModel.TenKh = "";
      this.customerId_Select = null;
    }
  }

  clearAreaList() {
    this.lstAreaList = [];
    this.objModel.MaKhuVuc = ""
    this.objModel.DiaChiGh = "";
    this.objModel.KhuVucGh = "";
  }
  //--------------------Event AreaList---------------------
  public AreaList_autocompleteHeaderTemplate = `
    <div class="header-row">
      <div class="col-5">Địa chỉ</div>
      <div class="col-3">Mã KV</div>
      <div class="col-3">KV giao hàng</div>
    </div>`;

  public AreaList_renderDataRowAutoComplete(data: CustomerDeliveryAddress): string {
    const html = `
      <div class="data-row">
        <div class="col-5 text-left">${data.DeliveryAdr == null ? "" : data.DeliveryAdr}</div>
        <div class="col-3">${data.MaKhuVuc == null ? "" : data.MaKhuVuc}</div>
        <div class="col-3">${data.TenKhuVuc == null ? "" : data.TenKhuVuc}</div>
      </div>`;
    return html;
  }

  AreaList_autocompleteCallback(event) {
    var item: CustomerDeliveryAddress = Object.assign({}, this.lstAreaList.find(x => x.Id == event));
    if (item.Id != null && item.Id != undefined) {
      this.objModel.DiaChiGh = item.DeliveryAdr;
      this.objModel.KhuVucGh = item.TenKhuVuc;
      this.objModel.MaKhuVuc = item.MaKhuVuc;
      this.reportDataResultCustomer();
      //backup
      this.objModelBackup.Cpgiao = this.objModel.Cpgiao;
    }
  }

  AreaList_onKeydown(event) {
    if (event.key === 'Enter') {
      //----
    }
  }

  GetAmounCost(objData: PriceSpreadsheets) {
    //Nếu số lượng < 500kg, chi phí giao = 2000 (VND/kg).
    if (objData.SoLuong < 500)
      objData.Cpgiao = 2000;

    //Nếu số lượng >= 500kg, chi phí giao = Giá vận chuyển / Số lượng (Giá vận chuyển được quy định ở Bảng giá vận chuyển).
    else {
      var MaSoLuong: string = "";
      //Nếu số lượng là số nguyên theo tấn (1000kg, 2000kg, v.v) sẽ lấy trực tiếp trong Bảng chọn mã số lượng.
      if (objData.SoLuong % 1000 == 0) {
        var soluongthat: number = objData.SoLuong;
        if (soluongthat > 13000)
          soluongthat = 13000;
        MaSoLuong = this.lstAmountDefine.find(x => x.SoLuong == soluongthat).MaSoLuong;
      }
      else {
        var soluongthat: number = Math.floor(objData.SoLuong / 1000 + 0.5) * 1000;
        MaSoLuong = this.lstAmountDefine.find(x => x.SoLuong == soluongthat).MaSoLuong;
      }

      //Lấy mã dò giá vận chuyển
      var MaDoGiaVC = objData.MaKhuVuc + (MaSoLuong.length == 1 ? "0" : "") + MaSoLuong;
      //Tìm giá vận chuyển
      var tem_giavc = this.lstShippedprice.find(x => x.MaDoGiaVc == MaDoGiaVC && x.StoreId == this.storeid);
      var GiaVC = tem_giavc == undefined || tem_giavc == null ? 0 : tem_giavc.GiaVc;
      //Tính chi phí giao
      objData.Cpgiao = Math.round(GiaVC / objData.SoLuong);
    }
  }

  //--------------------
  LoadData() {
    this.pricecalculatorService.getData().subscribe(
      {
        next: (res) => {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.lstDataResult = res.RepData;
            console.log(this.lstDataResult);
          }
        },
        error: (err) => {
          console.log(err);
        }
      }
    );
  }

  saveDataModel(isDuplycate: boolean) {
    if (this.objModel.MaHh == "" || this.objModel.MaHh == null || this.objModel.MaHh == undefined) {
      this.toastr.warning('Hãy chọn hàng hóa', 'Thông báo!');
    }
    else {
      if (this.lstDataResult.length <= 0)
        this.isNewModel = true;
      
      if(!this.isNewModel && this.objModel.StoreId != this.storeid){
        this.toastr.warning('Dữ liệu không cùng bảng tính', 'Thông báo!');
        return;
      }

      this.checkValueChange(isDuplycate);
      this.objModel.StoreId = this.storeid;
      this.pricecalculatorService.Insert(this.objModel, this.isNewModel).subscribe(res => {
        if (res !== undefined) {
          if (!res.IsOk) {
            this.toastr.error(res.MessageText, 'Thông báo!');
          } else {
            this.toastr.success(this.isNewModel ? 'Thêm dữ liệu thành công' : 'Dữ liệu đã được chỉnh sửa', 'Thông báo!');
            this.LoadData();
            this.clearModel();
          }
        } else {
          this.toastr.error('Lỗi xử lý hệ thống', 'Thông báo!');
        }
      }, err => {
        console.log(err);
      });
    }
  }

  deleteRowGird(Id: number) {

    this.pricecalculatorService.Delete(Id).subscribe(res => {
      if (res !== undefined) {
        if (!res.IsOk) {
          this.toastr.error(res.MessageText, 'Thông báo!');
        } else {
          this.toastr.warning('Dữ liệu đã được xóa', 'Thông báo!');
          this.LoadData();
        }
      } else {
        this.toastr.error("Lỗi xử lý hệ thống", 'Thông báo!');
      }
    }, err => {
      console.log(err);
    });
  }

  duplicateRowGird(index: number) {
    this.isNewModel = true;
    var item: PriceSpreadsheets = Object.assign({}, this.lstDataResult[index]);
    this.objModel = item;

    //reset list CIF
    var Product = this.lstProduct.find(x => x.ProductCode == this.objModel.MaHh);
    this.lstProductCIFPrice = Product.ListProductCIFPrice;


    this.saveDataModel(true);
  }

  clearModel() {
    this.productId_Select = null;
    this.customerId_Select = null;

    this.isNewModel = true;
    this.objModel = new PriceSpreadsheets();
    this.objPriced = new ProductDetail();
    this.objModel.CpquanLyTt = this.lstAggregateCosts.find(x => x.LoaiCp == "PhiQuanLyTrucTiep").ChiPhi;
    this.objModel.CpquanLyGt = this.lstAggregateCosts.find(x => x.LoaiCp == "PhiQuanLyGianTiep").ChiPhi;
    this.objModel.CptaiChinh = this.lstAggregateCosts.find(x => x.LoaiCp == "LaiSuatTB").ChiPhi;

    this.objModelBackup = new PriceSpreadsheets();
  }

  showDataRow(index: number) {
    this.isNewModel = false;
    this.objModel = Object.assign({}, this.lstDataResult[index]);
    this.objModel.CpquanLyGtTotal = Math.round(this.objModel.CpquanLyGtTotal);
    //reset list CIF
    var item = this.lstProduct.find(x => x.ProductCode == this.objModel.MaHh);
    this.productId_Select = item.ProductId;
    this.lstProductCIFPrice = item.ListProductCIFPrice;

    this.objModelBackup = Object.assign({}, this.lstDataResult[index]);
    this.GetAmounCost(this.objModelBackup);

    //---------Giá bán cũ------------
    this.objPriced = new ProductDetail();
    this.objPriced.GiaMin = item.GiaMin == null || item.GiaMin == undefined ? 0 : item.GiaMin;
    this.objPriced.KhMinName = item.KhMinName;
    this.objPriced.GiaMax = item.GiaMax == null || item.GiaMax == undefined ? 0 : item.GiaMax;
    this.objPriced.KhMaxName = item.KhMaxName;
    this.objPriced.GiaTb = item.GiaTb == null || item.GiaTb == undefined ? 0 : item.GiaTb;

  }

  openDialog(Id: number) {
    const modalRef = this.modalService.open(ComfirmDialogComponent, {
      backdrop: false, scrollable: true, centered: true
    });
    // xử lý sau khi đóng dialog, thực hiện load lại dữ liệu nếu muốn
    modalRef.result.then((result) => {
      if (result != undefined && result == true) {
        this.deleteRowGird(Id);
      }
    });
  }

  checkValueChange(isDuplycate: boolean) {

    var item: ProductDetail = Object.assign({}, this.lstProduct.find(x => x.ProductCode == this.objModel.MaHh));

    if (this.storeid == 1)
      this.objModel.IdColorCpveKho = this.objModel.CpveKho == item.CpveKhoLh ? 0 : 1;
    else
      this.objModel.IdColorCpveKho = this.objModel.CpveKho == item.CpveKhoBn ? 0 : 1;

    this.objModel.IdColorTgluuKhoTb = this.objModel.TgluuKhoTb == item.TgTonKhoTb ? 0 : 1;

    if (this.storeid == 1) {
      this.objModel.IdColorGiaCif = this.lstProductCIFPrice.filter(x => x.Cifsouth == this.objModel.GiaCif).length > 0 ? 0 : 1;
      this.objModel.IdColorTyGia = this.lstProductCIFPrice.filter(x => x.TyGia == this.objModel.TyGia && x.LoaiTien == this.objModel.LoaiTien).length > 0 ? 0 : 1;
    }

    else {
      this.objModel.IdColorGiaCif = this.lstProductCIFPrice.filter(x => x.Cifnorth == this.objModel.GiaCif).length > 0 ? 0 : 1;
      this.objModel.IdColorTyGia = this.lstProductCIFPrice.filter(x => x.TyGiaNorth == this.objModel.TyGia && x.LoaiTienNorth == this.objModel.LoaiTien).length > 0 ? 0 : 1;
    }

    this.objModel.IdColorSoLuong = this.objModel.SoLuong == 1000 ? 0 : 1;
    if (!isDuplycate)
      this.objModel.IdColorCpgiao = this.objModel.Cpgiao == this.objModelBackup.Cpgiao ? 0 : 1;
    else
      this.isNewModel = true;
    this.objModel.IdColorGiaBanDuKien = this.objModel.GiaBanDuKien == this.objModel.GiaBanHienHanh ? 0 : 1;
    this.objModel.IdColorCpquanLyGtTotal = this.objModel.CpquanLyGtTotal == (this.objModel.CpquanLyGt / 100) * this.objModel.GiaBanDuKien ? 0 : 1;
    this.objModel.IdColorCpbocXep = this.objModel.CpbocXep == this.lstAggregateCosts.find(x => x.LoaiCp == "PhiBocXep").ChiPhi ? 0 : 1;
    this.objModel.IdColorCpluuKho = this.objModel.CpluuKho == this.lstAggregateCosts.find(x => x.LoaiCp == "PhiLuuKho").ChiPhi ? 0 : 1;
    this.objModel.IdColorCpquanLyTt = this.objModel.CpquanLyTt == this.lstAggregateCosts.find(x => x.LoaiCp == "PhiQuanLyTrucTiep").ChiPhi ? 0 : 1;
    this.objModel.IdColorCpquanLyGt = this.objModel.CpquanLyGt == this.lstAggregateCosts.find(x => x.LoaiCp == "PhiQuanLyGianTiep").ChiPhi ? 0 : 1;
    this.objModel.IdColorCptaiChinh = this.objModel.CptaiChinh == this.lstAggregateCosts.find(x => x.LoaiCp == "LaiSuatTB").ChiPhi ? 0 : 1;

  }

  isNumberKey(evt: any, fieldName?: string) {
    this.objModel[fieldName] = 0;
    return Globalfunctions.isNumberKey(evt);
  }


}
