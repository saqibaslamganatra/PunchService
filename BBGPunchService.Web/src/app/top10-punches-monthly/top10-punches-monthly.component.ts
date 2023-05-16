import { Component, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as Highcharts from 'highcharts';
import { Options, SeriesBarOptions, SeriesLineOptions, SeriesAreaOptions, SeriesPieOptions } from 'highcharts';
import { environment } from 'src/environments/environment.development';


import ExportingModule from 'highcharts/modules/exporting';
import ExportDataModule from 'highcharts/modules/export-data';


ExportingModule(Highcharts);
ExportDataModule(Highcharts);


@Component({
  selector: 'app-top10-punches-monthly',
  templateUrl: './top10-punches-monthly.component.html',
  styleUrls: ['./top10-punches-monthly.component.scss']
})
export class Top10PunchesMonthlyComponent implements OnInit, AfterViewInit {

  punchData: any[] = [];
  chartOptions: Options = {};
  selectedChartType: string = 'column';

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getTop10PunchesMonthly();
  }

  ngAfterViewInit(): void {
    this.generateChart();
  }

  getTop10PunchesMonthly(): void {
    const url = environment.apiUrl+'/api/Dashboard/top10-punches-monthly';
    this.http.get<any[]>(url).subscribe(data => {
      this.punchData = data;
      this.generateChart();
    });
  }

  generateChart(): void {
    const categories: string[] = this.punchData.map(punch => punch.employeeName);
    const defaultColors = ['#7cb5ec', '#434348', '#90ed7d', '#f7a35c', '#8085e9', '#f15c80', '#e4d354', '#2b908f', '#f45b5b', '#91e8e1'];
    const seriesData: Highcharts.PointOptionsObject[] = this.punchData.map((punch, index) => ({
      name: punch.employeeName,
      y: punch.punchesCount,
      color: Highcharts.getOptions().colors?.[index % 10] || defaultColors[index % 10]
    }));

    // Set chart options
    this.chartOptions = {
      chart: {
        type: this.selectedChartType
      },
      exporting: {
        enabled: true,
        buttons: {
          contextButton: {
            menuItems: ['downloadPNG', 'downloadJPEG', 'downloadPDF', 'downloadSVG', 'separator', 'downloadXLS', 'downloadCSV']
          }
        }
      },
      title: {
        text: 'Top 10 Punches Monthly'
      },
      xAxis: {
        categories: categories,
        title: {
          text: 'Employees'
        }
      },
      yAxis: {
        min: 0,
        title: {
          text: 'Punch Count'
        }
      },
      series: [
        {
          name: 'Punches',
          data: seriesData,
          type: this.selectedChartType === 'pie' ? 'pie' : undefined
        } as SeriesBarOptions | SeriesLineOptions | SeriesAreaOptions | SeriesPieOptions
      ]
    };

    // Render the chart
    Highcharts.chart('chart-container', this.chartOptions);
  }
}

