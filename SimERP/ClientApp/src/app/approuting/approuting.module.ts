import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { HomeLayoutComponent } from '../systems/layouts/home-layout.component';
import { AuthGuard } from '../systems/authguard';
import { LoginLayoutComponent } from '../systems/layouts/login-layout.component';
import { LoginComponent } from '../systems/login/login.component';
import { PagenotfoundComponent } from '../common/pagenotfound/pagenotfound.component';
import { UserComponent } from '../systems/user/user.component';
import { LogoutComponent } from '../systems/logout/logout.component';
import { FirstchangepasswordComponent } from '../systems/firstchangepassword/firstchangepassword.component';
import { PagelistComponent } from '../lists/pagelist/pagelist/pagelist.component';
import { RolelistComponent } from '../lists/rolelist/rolelist/rolelist.component';
import { UserpermissionComponent } from '../systems/userpermission/userpermission/userpermission.component';
import { TaxComponent } from '../lists/taxcomponent/tax/tax.component';
import { AggregatecostsComponent } from '../lists/aggregatecosts/aggregatecosts/aggregatecosts.component';
import { ArealistComponent } from '../lists/area/arealist/arealist.component';
import { ProductlistComponent } from '../lists/product/productlist/productlist.component';
import { ShippedpriceComponent } from '../lists/shippedprice/shippedprice/shippedprice.component';
import { CustomerdetailComponent } from '../lists/customerdetail/customerdetail/customerdetail.component';
import { PricecalculatorComponent } from '../calculator/pricecalculator/pricecalculator/pricecalculator.component';
import {ImportdataComponent} from '../lists/importdata/importdata.component';
import {ReportmainComponent} from '../datafilter/reportmain/reportmain.component';


const routes: Routes = [
  {
    path: '', component: HomeLayoutComponent, canActivate: [AuthGuard], children: [
      { path: 'user', component: UserComponent },
      { path: 'pagelist', component: PagelistComponent },
      { path: 'rolelist', component: RolelistComponent },
      { path: 'userpermission', component: UserpermissionComponent },
      { path: 'datafilterimport', component: ReportmainComponent },
    ]
  },
  {
    path: '', component: LoginLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'quit', component: LogoutComponent },
      { path: 'firstchangepassword', component: FirstchangepasswordComponent }
    ]
  },
  { path: '**', component: PagenotfoundComponent },

];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forRoot(routes),
  ],
  exports: [RouterModule]
})
export class ApproutingModule {
}
