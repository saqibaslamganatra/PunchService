//punching-data.component.ts
import { Component, OnInit, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { HttpClient } from '@angular/common/http';
import { NgxSpinnerService } from 'ngx-spinner';
import { MatSnackBar, MatSnackBarConfig, MatSnackBarHorizontalPosition, MatSnackBarVerticalPosition } from '@angular/material/snack-bar';
import { DateAdapter } from '@angular/material/core';
import { environment } from 'src/environments/environment.development';

export interface PunchingData {
  enrolNo: string;
  fullName: string;
  punchDateTime: string;
  punchDirection: string;
  punchNumber: number;
  oid: string;
}

export interface ApiResponse {
  result: PunchingData[];
  pagination: {
    totalPages: number;
    currentPage: number;
    pageSize: number;
    totalItems: number;
  };
}

@Component({
  selector: 'app-punching-data',
  templateUrl: './punching-data.component.html',
  styleUrls: ['./punching-data.component.scss']
})
export class PunchingDataComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['enrolNo', 'fullName', 'punchDateTime', 'punchDirection', 'punchNumber'];
  dataSource: MatTableDataSource<PunchingData> = new MatTableDataSource<PunchingData>([]);
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  isLoading = false;
  startDate: Date | null = null;
  endDate: Date | null = null;
  search: string | null = null;
  pageSize = 10;
  pageIndex = 0;
  totalItems = 0;
  horizontalPosition: MatSnackBarHorizontalPosition = 'center';
  verticalPosition: MatSnackBarVerticalPosition = 'top';
  action: boolean = true;
  actionButtonLabel: string = 'Retry';

  constructor(private http: HttpClient,
    private spinner: NgxSpinnerService,
    private matPaginatorIntl: MatPaginatorIntl,
    private cdRef: ChangeDetectorRef,
    private snackBar: MatSnackBar,
    private dateAdapter: DateAdapter<Date>) {
    // set paginator labels
    this.matPaginatorIntl.itemsPerPageLabel = 'Items per page:';
    this.matPaginatorIntl.nextPageLabel = 'Next';
    this.matPaginatorIntl.previousPageLabel = 'Previous';
    this.matPaginatorIntl.firstPageLabel = 'First';
    this.matPaginatorIntl.lastPageLabel = 'Last';
    this.dateAdapter.setLocale('en-GB'); //dd/MM/yyyy
  }

  ngOnInit() {
    this.isLoading = true;
    this.spinner.show();
    this.getPunchData();
  }

  ngAfterViewInit() {
    this.updatePaginatorRangeLabel();
    this.paginator.page.subscribe(() => {
      this.pageIndex = this.paginator.pageIndex;
      this.pageSize = this.paginator.pageSize;
      this.getPunchData();
    });
  }

  applyFilter() {
    this.isLoading = true;
    this.spinner.show();
     let config = new MatSnackBarConfig();
    config.horizontalPosition = this.horizontalPosition;
    config.verticalPosition = this.verticalPosition;
    if (this.startDate && this.endDate && this.startDate > this.endDate) {
      this.snackBar.open("Start date cannot be greater than end date.", this.action ? this.actionButtonLabel : undefined, config);
      return;
    }
    this.pageIndex = 0;
    this.getPunchData();
    this.isLoading = false;
    this.spinner.hide();
  }
  
  exportTable(type: string) {
    this.spinner.show();
    this.isLoading = true;
    const body = {
      pageSize: this.pageSize,
      pageNumber: this.pageIndex + 1,
      startDate: this.startDate ? this.formatDate(this.startDate) : null,
      endDate: this.endDate ? this.formatDate(this.endDate) : null,
      searchText: this.search
    };
    const exportUrl = environment.apiUrl + '/api/PunchingData/Export?type=' + type;
    const exportFileName = `punch-data-${new Date().toLocaleDateString()}.${type}`;
    this.http.post(exportUrl, body, { responseType: 'blob' }).subscribe((response: any) => {
      const blob = new Blob([response], { type: `application/${type}` });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = exportFileName;
      link.click();
      window.URL.revokeObjectURL(url);
      this.spinner.hide();
      this.isLoading = false;
    }, (error: any) => {
      console.error(error);
      this.spinner.hide();
      this.isLoading = false;
    });
  }
  
   

  getPunchData() {
    const body = {
      pageSize: this.pageSize,
      pageNumber: this.pageIndex + 1,
      startDate: this.startDate ? this.formatDate(this.startDate) : null,
      endDate: this.endDate ? this.formatDate(this.endDate) : null,
      searchText: this.search
    };
    this.http.post<ApiResponse>(environment.apiUrl+'/api/PunchingData', body).subscribe(
      response => {
        this.dataSource.data = response.result;
        this.dataSource.sort = this.sort;
        this.totalItems = response.pagination.totalItems;
        this.updatePaginatorRangeLabel();
        this.spinner.hide();
        this.isLoading = false;
      },
      error => {
        console.error(error);
        this.spinner.hide();
        this.isLoading = false;
      }
    );
  }
  
  
  formatDate(date: Date): string {
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear().toString();
    return `${day}/${month}/${year}`;
  }

  private updatePaginatorRangeLabel(): void {
    const start = this.paginator.pageIndex * this.paginator.pageSize + 1;
    const end = Math.min((this.paginator.pageIndex + 1) * this.paginator.pageSize, this.totalItems);
    const totalItems = this.totalItems > this.pageSize ? this.totalItems : this.pageSize;
    this.matPaginatorIntl.changes.next();
    this.matPaginatorIntl.getRangeLabel = (page: number, pageSize: number, length: number) => {
      return `${start} - ${end} of ${totalItems}`;
    };
    this.cdRef.detectChanges();
  }


  onPageChange(event: any) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.getPunchData();
  }
}
