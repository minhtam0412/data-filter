import {Inject, Injectable} from '@angular/core';
import {AuthenParams} from '../../common/commomodel/AuthenParams';
import {ReqListSearch} from '../../common/commomodel/ReqListSearch';
import {ReqListAdd} from '../../common/commomodel/ReqListAdd';
import {ReqListDelete} from '../../common/commomodel/ReqListDelete';
import {ReqListUpdateSortOrder} from '../../common/commomodel/ReqListUpdateSortOrder';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {ResponeResult} from '../../common/commomodel/ResponeResult';
import {ProductDetail} from './model/productdetail';
import { ROOT_URL } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  authenParams = new AuthenParams();
  reqListSearch = new ReqListSearch();
  reqListAdd = new ReqListAdd();
  reqListDelete = new ReqListDelete();
  reqListUpdateSortOrder = new ReqListUpdateSortOrder();

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.baseUrl = ROOT_URL;
  }

  getData(searchString?: string, isActive?: any, startRow?: number, maxRow?: number) {
    this.reqListSearch.AuthenParams = this.authenParams;
    this.reqListSearch.MaxRow = maxRow;
    this.reqListSearch.IsActive = isActive;
    this.reqListSearch.StartRow = startRow;
    this.reqListSearch.SearchString = searchString;

    const jsonString = JSON.stringify(this.reqListSearch);
    const headers = new HttpHeaders().set('content-type', 'application/json');
    return this.httpClient.post<ResponeResult>(this.baseUrl + 'api/list/productdetail', jsonString, {headers});
  }

  deleteData(rowData: ProductDetail) {
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.reqListDelete.AuthenParams = this.authenParams;
    let Id = rowData.Id;
    if (rowData.ProductId > 0) {
      Id = rowData.ProductId;
    }
    this.reqListDelete.ID = rowData.Id;
    const jsonString = JSON.stringify(this.reqListDelete);
    return this.httpClient.post<ResponeResult>(this.baseUrl + 'api/list/deleteproductdetail', jsonString, {headers});
  }

  saveData(rowData: ProductDetail, isNew: boolean) {
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.reqListAdd.AuthenParams = this.authenParams;
    this.reqListAdd.RowData = rowData;
    this.reqListAdd.IsNew = isNew;
    const jsonString = JSON.stringify(this.reqListAdd);
    return this.httpClient.post<ResponeResult>(this.baseUrl + 'api/list/saveproductdetail', jsonString, {headers});
  }

  updateSortOrder(upID: number, downID: number) {
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.reqListUpdateSortOrder.AuthenParams = this.authenParams;
    this.reqListUpdateSortOrder.UpID = upID;
    this.reqListUpdateSortOrder.DownID = downID;
    const jsonString = this.reqListUpdateSortOrder;
    return this.httpClient.post<ResponeResult>(this.baseUrl + '/api/list/updatesortproductdetail', jsonString, {headers});
  }

  getInfo(id: number) {
    const headers = new HttpHeaders().set('content-type', 'application/json');
    this.reqListSearch = new ReqListSearch();
    this.reqListSearch.AuthenParams = this.authenParams;
    this.reqListSearch.SearchString = String(id);
    return this.httpClient.post<ResponeResult>(this.baseUrl + 'api/list/getproductdetailinfo', JSON.stringify(this.reqListSearch),
      {headers});
  }
}
