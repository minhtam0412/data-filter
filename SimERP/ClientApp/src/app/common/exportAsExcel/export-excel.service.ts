import { Injectable } from '@angular/core';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';
import { ReportTotal } from 'src/app/datafilter/model/reporttotal';

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';

@Injectable({
  providedIn: 'root'
})
export class ExportExcelService {

  constructor() { }

  public exportAsExcelFile(lstHeader: string[], lstDataResult: ReportTotal[], lstIndexActive: number[], excelFileName: string): void {

    const lstReport = [];
    lstReport.push(this.parseDataHeader(lstHeader, lstIndexActive));
    this.parseDataRow(lstDataResult, lstIndexActive, lstReport);

    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(lstReport);

    var range = XLSX.utils.decode_range(worksheet['!ref']);
    for (var C = range.s.c; C <= range.e.c; ++C) {
      var address = XLSX.utils.encode_col(C) + "1"; // <-- first row, column number C
      if (!worksheet[address]) continue;
      worksheet[address].v = "";
    }

    const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    //const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'buffer' });

    this.saveAsExcelFile(excelBuffer, excelFileName);
  }

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE
    });
    FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
  }

  parseDataHeader(lstHeader: string[], lstIndexActive: number[]) {
    var lstTem: string[] = [];
    for (let i = 0; i < lstIndexActive.length; ++i) {
      lstTem.push(lstHeader[lstIndexActive[i]]);
    }
    return lstTem;
  }

  parseDataRow(lstDataResult: ReportTotal[], lstIndexActive: number[], lstReport) {
    for (let i = 0; i < lstDataResult.length; ++i) {
      let item: ReportTotal = lstDataResult[i];
      var row = [item.PhanLoai, item.NamNhap, item.NgayNhap,
      item.ChiCucHaiQuan, item.CangXuatNhap, item.TenLoHang, item.MaDoanhNghiep, item.DonViDoiTac, item.HsCode, item.ChungLoaiHangHoaXuatNhap,
      item.DoanhNghiepXuatNhap, item.NuocXuatXu, item.Dvt, item.Luong, item.SoLuongQuyDoi, item.DonGia, item.DonGiaUSD, item.TriGia, item.NgoaiTeThanhToan, item.TyGiaVnd, item.TriGiaVnd, item.TyGiaUsd, item.TriGiaUsd,
      item.DieuKienGiaoHang, item.DieuKienThanhToan, item.Tsxnk, item.ThueXnk, item.Tsttdb, item.ThueTtdb, item.Tsvat, item.ThueVat, item.PhuThu, item.MienThue,
      item.PhuongTienVanTai, item.TenPhuongTienVanTai, item.NuocXuatKhau, item.NuocNhapKhau, item.CangNuocNgoai, item.PhanLoaiTrangThai, item.SoToKhai
      ];

      var row_tem = [];
      for (let i = 0; i < lstIndexActive.length; ++i) {
        row_tem.push(row[lstIndexActive[i]]);
      }

      lstReport.push(row_tem);
    };
  }
}
