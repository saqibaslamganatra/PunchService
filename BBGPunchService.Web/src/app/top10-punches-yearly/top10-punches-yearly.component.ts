import { Component, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as Highcharts from 'highcharts';
import { Options, SeriesColumnOptions, SeriesSplineOptions, SeriesPieOptions } from 'highcharts';
import { environment } from 'src/environments/environment.development';

import ExportingModule from 'highcharts/modules/exporting';
import ExportDataModule from 'highcharts/modules/export-data';

ExportingModule(Highcharts);
ExportDataModule(Highcharts);

@Component({
  selector: 'app-top10-punches-yearly',
  templateUrl: './top10-punches-yearly.component.html',
  styleUrls: ['./top10-punches-yearly.component.scss']
})
export class Top10PunchesYearlyComponent implements OnInit, AfterViewInit {

  punchData: any[] = [];
  combinedChartOptions: Options = {};
  selectedChartType: string = 'pie';

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getTop10PunchersYearly();
  }

  ngAfterViewInit(): void {
    this.generateCharts();
  }

  getTop10PunchersYearly(): void {
    const url = environment.apiUrl+'/api/Dashboard/top10-punches-yearly';
    this.http.get<any[]>(url).subscribe(data => {
      this.punchData = data;
      this.generateCharts();
    });
  }

  generateCharts(): void {
    const categories: string[] = this.punchData.map(punch => punch.employeeName);
    const seriesData: Highcharts.PointOptionsObject[] = this.punchData.map((punch, index) => ({
      y: punch.punchesCount,
      color: Highcharts.getOptions().colors?.[index % 10] || '#7cb5ec'
    }));
    const seriesPieData: Highcharts.PointOptionsObject[] = this.punchData.map((punch) => ({
      name: punch.employeeName,
      y: punch.punchesCount
    }));

    this.combinedChartOptions = {
      title: {
        text: 'Top 10 Punches Yearly',
        align: 'left'
      },
      exporting: {
        enabled: true,
        buttons: {
          contextButton: {
            menuItems: ['downloadPNG', 'downloadJPEG', 'downloadPDF', 'downloadSVG', 'separator', 'downloadXLS', 'downloadCSV']
          }
        }
      },
      xAxis: {
        categories: categories
      },
      yAxis: {
        title: {
          text: 'Punch Count'
        }
      },
      tooltip: {
        valueSuffix: ' punches'
      },
      series: [
        {
          type: this.selectedChartType as any,
          name: '2020',
          data: seriesData,
          center: [75, 65],
          innerSize: '70%',
          showInLegend: false,
          dataLabels: {
            enabled: false
          },         
        } as SeriesColumnOptions,
        {
          type: 'spline',
          name: 'Average',
          data: seriesData,
          marker: {
            lineWidth: 2,
            lineColor: (Highcharts.getOptions().colors ?? ['#7cb5ec'])[3],
            fillColor: 'white'
          },
          visible: this.selectedChartType !== 'spline',
        } as SeriesSplineOptions,
        {          
          name: 'Total',
          type: 'pie',
          data: seriesPieData,
          size: 200,              
          visible: this.selectedChartType === 'pie',
        }as SeriesPieOptions,
      ]
    };

    Highcharts.chart('combined-chart-container', this.combinedChartOptions);
  }
}
