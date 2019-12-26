import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';
import {AppComponent} from './app.component';
import {HeaderComponent} from './header/header.component';
import {MDBBootstrapModule} from 'angular-bootstrap-md';
import {PaginationComponent} from './pagination/pagination.component';
import {NgbDatepickerModule, NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {ToastrModule} from 'ngx-toastr';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ComfirmDialogComponent} from './common/comfirm-dialog/comfirm-dialog.component';
import {LoginComponent} from './systems/login/login.component';
import {AuthGuard} from './systems/authguard';
import {HomeLayoutComponent} from './systems/layouts/home-layout.component';
import {LoginLayoutComponent} from './systems/layouts/login-layout.component';
import {HomeComponent} from './home/home.component';
import {ApproutingModule} from './approuting/approuting.module';
import {PagenotfoundComponent} from './common/pagenotfound/pagenotfound.component';
import {UserComponent} from './systems/user/user.component';
import {JwtModule} from '@auth0/angular-jwt';
import {Key_UserInfo} from './common/config/globalconfig';
import {JwtInterceptor} from './systems/login/jwtinterceptor';
import {LogoutComponent} from './systems/logout/logout.component';
import {ChangepasswordComponent} from './systems/changepassword/changepassword.component';
import {FirstchangepasswordComponent} from './systems/firstchangepassword/firstchangepassword.component';
import {LoadingComponent} from './common/loading/loading.component';
import {CookieService} from 'ngx-cookie-service';
import {CheckpermissionDirective} from './common/checkpermission/checkpermission.directive';
import {PagelistComponent} from './lists/pagelist/pagelist/pagelist.component';
import {RolelistComponent} from './lists/rolelist/rolelist/rolelist.component';
import {NgSelectModule} from '@ng-select/ng-select';
import {UserpermissionComponent} from './systems/userpermission/userpermission/userpermission.component';
import {CheckmenupermissionDirective} from './common/checkpermission/checkmenupermission.directive';
import {EncrDecrService} from './common/security/encr-decr.service';
import {registerLocaleData} from '@angular/common';
import vi from '@angular/common/locales/vi';
import {BsDatepickerModule} from 'ngx-bootstrap';
// đăng ký locale cho control datetime picker
import {defineLocale} from 'ngx-bootstrap/chronos';
import {viLocale} from 'ngx-bootstrap/locale';
import {LibLocaleModule} from './common/locale/lib-locale/lib-locale.module';
import {NgxBarcodeModule} from 'ngx-barcode';
import {TaxComponent} from './lists/taxcomponent/tax/tax.component';
import {NguiAutoCompleteModule} from './common/auto-complete/auto-complete.module';
import {AppFormatNumberDirective} from './common/locale/pipes/app-format-number.directive';
import {AggregatecostsComponent} from './lists/aggregatecosts/aggregatecosts/aggregatecosts.component';
import {TaxinfoComponent} from './lists/taxcomponent/taxinfo/taxinfo.component';
import {FormatnumberPipe} from './common/locale/pipes/formatnumber.pipe';
import {AreadetailComponent} from './lists/area/areadetail/areadetail.component';
import {ProductlistComponent} from './lists/product/productlist/productlist.component';
import {ProductdetailComponent} from './lists/product/productdetail/productdetail.component';
import {ShippedpriceComponent} from './lists/shippedprice/shippedprice/shippedprice.component';
import {ArealistComponent} from './lists/area/arealist/arealist.component';
import {CustomerdetailComponent} from './lists/customerdetail/customerdetail/customerdetail.component';
import {PricecalculatorComponent} from './calculator/pricecalculator/pricecalculator/pricecalculator.component';
import {ImportdataComponent} from './lists/importdata/importdata.component';
import {ReportmainComponent} from './datafilter/reportmain/reportmain.component';
import {NgMultiSelectDropDownModule} from 'ng-multiselect-dropdown';
import {ReportpreviewComponent} from './datafilter/reportpreview/reportpreview.component';
import {QuantitytranformComponent} from './datafilter/quantitytranform/quantitytranform.component';
import {UnitpricetranformComponent} from './datafilter/unitpricetranform/unitpricetranform.component';
import {ScrollingModule} from '@angular/cdk/scrolling';
import {NgxDatatableModule} from '@swimlane/ngx-datatable';
import {AngularSlickgridModule} from 'angular-slickgrid';

registerLocaleData(vi);

defineLocale('vi', viLocale);

export function tokenGetter() {
  const objToken = JSON.parse(localStorage.getItem(Key_UserInfo));
  return objToken['access_token'];
}

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    PaginationComponent,
    ComfirmDialogComponent,
    LoginComponent,
    HomeLayoutComponent,
    LoginLayoutComponent,
    HomeComponent,
    PagenotfoundComponent,
    UserComponent,
    LogoutComponent,
    ChangepasswordComponent,
    FirstchangepasswordComponent,
    LoadingComponent,
    CheckpermissionDirective,
    CheckmenupermissionDirective,
    PagelistComponent,
    RolelistComponent,
    UserpermissionComponent,
    UserpermissionComponent,
    TaxComponent,
    TaxinfoComponent,
    FormatnumberPipe,
    AppFormatNumberDirective,
    TaxinfoComponent,
    ArealistComponent,
    AreadetailComponent,
    ProductlistComponent,
    ProductdetailComponent,
    ShippedpriceComponent,
    AggregatecostsComponent,
    CustomerdetailComponent,
    PricecalculatorComponent,
    ImportdataComponent,
    ReportmainComponent,
    ReportpreviewComponent,
    QuantitytranformComponent,
    UnitpricetranformComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    ToastrModule.forRoot(),
    BrowserAnimationsModule,
    MDBBootstrapModule.forRoot(),
    NgbModule,
    NgbDatepickerModule,
    ReactiveFormsModule,
    // NguiAutoCompleteModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    RouterModule,
    ApproutingModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: [''],
        blacklistedRoutes: ['']
      }
    }),
    NgSelectModule,
    BsDatepickerModule.forRoot(),
    LibLocaleModule,
    NgxBarcodeModule,
    NguiAutoCompleteModule,
    NgMultiSelectDropDownModule.forRoot(),
    NgxDatatableModule,
    AngularSlickgridModule.forRoot()
  ],
  providers: [AuthGuard,
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    CookieService,
    EncrDecrService,
  ],
  bootstrap: [AppComponent],
  entryComponents:
    [
      ComfirmDialogComponent,
      ChangepasswordComponent,
      ReportpreviewComponent,
      UnitpricetranformComponent,
      QuantitytranformComponent,
    ],

})

export class AppModule {
}
