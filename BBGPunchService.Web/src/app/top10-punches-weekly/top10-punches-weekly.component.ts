// top10-punches-weekly.component.ts
import { Component, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as Highcharts from 'highcharts';
import { Options, SeriesOptionsType, SeriesPieOptions } from 'highcharts';
import { FormsModule } from '@angular/forms';
import { environment } from 'src/environments/environment.development';

import ExportingModule from 'highcharts/modules/exporting';
import ExportDataModule from 'highcharts/modules/export-data';

ExportingModule(Highcharts);
ExportDataModule(Highcharts);

@Component({
  selector: 'app-top10-punches-weekly',
  templateUrl: './top10-punches-weekly.component.html',
  styleUrls: ['./top10-punches-weekly.component.scss']
})
export class Top10PunchesWeeklyComponent implements OnInit, AfterViewInit {

  punchData: any[] = [];
  chartOptions: Options = {};
  selectedChartType: string = 'pie'; // Add this line

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getTop10PunchesWeekly();
  }

  ngAfterViewInit(): void {
    this.generateChart();
  }

  getTop10PunchesWeekly(): void {
    const url = environment.apiUrl+'/api/Dashboard/top10-punches-weekly';
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
        type: this.selectedChartType // Update this line
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
        text: 'Top 10 Punches Weekly'
      },
      tooltip: {
        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
      },
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: true,
            format: '<b>{point.name}</b>: {point.percentage:.1f} %'
            },
            showInLegend: true
            }
            },
            xAxis: {
            categories: categories,
            title: {
            text: 'Employees'
            },
            visible: this.selectedChartType !== 'pie'
            },
            yAxis: {
            min: 0,
            title: {
            text: 'Punch Count'
            },
            visible: this.selectedChartType !== 'pie'
            },
            series: [
            {
            name: 'Punches',
            data: seriesData,
            type: this.selectedChartType
            } as SeriesOptionsType
            ]
            };
            // Render the chart
Highcharts.chart('chart-containerPie', this.chartOptions);
}

}
