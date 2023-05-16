//top10-punches-quarterly.component.ts
import { Component, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as Highcharts from 'highcharts';
import { Options, SeriesOptionsType, SeriesLineOptions, SeriesColumnOptions, SeriesBarOptions, SeriesAreaOptions, SeriesPieOptions } from 'highcharts';
import { environment } from 'src/environments/environment.development';

import ExportingModule from 'highcharts/modules/exporting';
import ExportDataModule from 'highcharts/modules/export-data';

ExportingModule(Highcharts);
ExportDataModule(Highcharts);


@Component({
  selector: 'app-top10-punches-quarterly',
  templateUrl: './top10-punches-quarterly.component.html',
  styleUrls: ['./top10-punches-quarterly.component.scss']
})
export class Top10PunchesQuarterlyComponent implements OnInit, AfterViewInit {

  punchData: any[] = [];
  chartOptions: Options = {};
  selectedChartType: string = 'line';

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getTop10PunchesQuarterly();
  }

  ngAfterViewInit(): void {
    this.generateChart();
  }

  getTop10PunchesQuarterly(): void {
    const url = environment.apiUrl+'/api/Dashboard/top10-punches-quarterly';
    this.http.get<any[]>(url).subscribe(data => {
      this.punchData = data;
      this.generateChart();
    });
  }

  generateChart(): void {
    const categories: string[] = this.punchData.map(punch => punch.employeeName);
    const seriesLineData: number[] = this.punchData.map(punch => punch.punchesCount);
    const seriesColumnData: number[] = this.punchData.map(punch => punch.punchesCount / 10);
    const seriesPieData: Highcharts.PointOptionsObject[] = this.punchData.map((punch) => ({
      name: punch.employeeName,
      y: punch.punchesCount
    }));
    const defaultColors = ['#7cb5ec', '#434348', '#90ed7d', '#f7a35c', '#8085e9', '#f15c80', '#e4d354', '#2b908f', '#f45b5b', '#91e8e1'];
    const series: SeriesOptionsType[] = [
      {
        name: 'Punches',
        type: this.selectedChartType,
        yAxis: 1,
        data: seriesLineData,
        marker: {
          enabled: false
        },
      } as SeriesLineOptions,
      {
        name: 'Punches (normalized)',
        type: this.selectedChartType, 
        yAxis: 0,
        data: seriesColumnData,
        color: defaultColors[1]
      },
      {
        name: 'Punches',
        type: 'pie',
        data: seriesPieData,
      } as SeriesPieOptions
    ] as SeriesLineOptions[] | SeriesColumnOptions[] | SeriesBarOptions[] | SeriesAreaOptions[] | SeriesPieOptions[];
  
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
        text: 'Top 10 Punches Quarterly'
      },
      xAxis: {
        categories: categories,
        title: {
          text: 'Employees'
        }
      },
      yAxis: [
        {
          min: 0,
          title: {
            text: 'Punch Count (normalized)'
          }
        },
        {
          title: {
            text: 'Punch Count'
          },
          opposite: true
        }
      ],
      series: series
    };
  
    // Render the chart
    Highcharts.chart('chart-containerQ', this.chartOptions);
  }
  
}  
