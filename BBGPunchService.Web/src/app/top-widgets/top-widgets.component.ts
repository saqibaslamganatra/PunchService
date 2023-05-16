// top-widgets.component.ts
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { NgxSpinnerService } from 'ngx-spinner';
import {
  faLocation,
  faShop,
  faBoxes,
  faMoneyBill,
} from '@fortawesome/free-solid-svg-icons';
import { environment } from 'src/environments/environment.development';
interface Widget {
  icon: IconProp;
  top: string;
  bottom: string;
  color: string;
}

@Component({
  selector: 'app-top-widgets',
  templateUrl: './top-widgets.component.html',
  styleUrls: ['./top-widgets.component.scss']
})
export class TopWidgetsComponent implements OnInit {

  widgets?: Widget[];

  faLocation = faLocation;
  faShop = faShop;
  faBoxes = faBoxes;
  faMoneyBill = faMoneyBill;

  constructor(private spinner: NgxSpinnerService , private http: HttpClient) { }

  ngOnInit(): void {
    this.spinner.show();
    this.http.get<Widget[]>(environment.apiUrl+'/api/Dashboard/top-widgets').subscribe(
      (data) => { 
        this.widgets = data.map(widget => {
          switch (widget.icon) {
            case 'faLocation':
              widget.icon = faLocation;
              break;
            case 'faShop':
              widget.icon = faShop;
              break;
            case 'faBoxes':
              widget.icon = faBoxes;
              break;
            case 'faMoneyBill':
              widget.icon = faMoneyBill;
              break;
            default:
              widget.icon = faLocation; 
          }
          return widget;
        });
        this.spinner.hide();
      },
      (error) => { console.error(error); }
    );
  }

}
