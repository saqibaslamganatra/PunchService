import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { AppComponent } from './app.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { NgxSpinnerModule } from 'ngx-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon'; 
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { DashboardComponent } from './dashboard/dashboard.component';
import { PunchingDataComponent } from './punching-data/punching-data.component';
import { RouterModule } from '@angular/router'; 
import { MatTooltipModule } from '@angular/material/tooltip';
import { AppRoutingModule } from './app-routing.module';
import { SidebarComponent } from './sidebar/sidebar.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';


import { TopWidgetsComponent } from './top-widgets/top-widgets.component';

import { MatTabsModule } from '@angular/material/tabs';

import { FlexLayoutModule } from '@angular/flex-layout';
import { MatGridListModule } from '@angular/material/grid-list';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { MatMenuModule } from '@angular/material/menu';
import { Top10PunchesWeeklyComponent } from './top10-punches-weekly/top10-punches-weekly.component';
import { Top10PunchesMonthlyComponent } from './top10-punches-monthly/top10-punches-monthly.component';
import { Top10PunchesQuarterlyComponent } from './top10-punches-quarterly/top10-punches-quarterly.component';
import { Top10PunchesYearlyComponent } from './top10-punches-yearly/top10-punches-yearly.component';
import { HighchartsChartModule } from 'highcharts-angular';



import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    PunchingDataComponent,
    HeaderComponent,
    SidebarComponent,
    FooterComponent,
    TopWidgetsComponent,
    Top10PunchesWeeklyComponent,
    Top10PunchesMonthlyComponent,
    Top10PunchesQuarterlyComponent,
    Top10PunchesYearlyComponent,
    Top10PunchesQuarterlyComponent,
 
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatCardModule,
    MatInputModule,
    MatDatepickerModule,
    MatDialogModule,
    MatNativeDateModule,
    NgxSpinnerModule,
    MatSnackBarModule,
    MatIconModule,
    MatToolbarModule,
    MatButtonModule, 
    RouterModule ,
    MatTooltipModule,
    AppRoutingModule,
    MatSidenavModule,
    MatListModule,
    MatCardModule,
    MatButtonModule,
    MatTabsModule,
    FlexLayoutModule,
    MatGridListModule,
    FontAwesomeModule,
    MatMenuModule,
    HighchartsChartModule,
    CommonModule

  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
