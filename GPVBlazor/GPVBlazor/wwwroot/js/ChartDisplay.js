export class PieChart {
    static createPieChart(chartModelJSON) {
        try {
            const chartData = JSON.parse(chartModelJSON);

            if (!this.validateChartData(chartData)) throw new Error('Invalid chart data');

            const ctx = document.getElementById(chartData.chartId);
            if (!ctx) {
                console.error(`Element with id ${chartData.chartId} not found.`);
                return;
            }

            // Add chart container class for CSS targeting
            const canvasParent = ctx.parentElement;
            if (canvasParent) {
                canvasParent.classList.add('chart-container');
            }

            const existingChart = Chart.getChart(ctx);
            if (existingChart) existingChart.destroy();

            // Configure dataset based on chart type
            const datasets = chartData.datasets.map(dataset => {
                // Line chart specific configuration
                if (chartData.type === 'line') {
                    return {
                        label: chartData.label,
                        data: dataset.data,
                        borderColor: '#ffffff',
                        backgroundColor: 'rgba(255, 255, 255, 0.1)',
                        borderWidth: 4,
                        fill: false,
                        tension: 0.4,
                        pointRadius: 6,
                        pointHoverRadius: 8,
                        pointBackgroundColor: '#ffffff',
                        pointBorderColor: '#000000',
                        pointBorderWidth: 3
                    };
                }

                // Default configuration for other chart types
                return {
                    label: chartData.label,
                    data: dataset.data,
                    backgroundColor: dataset.backgroundColor,
                    borderWidth: 1,
                    hoverOffset: 4
                };
            });

            const chart = new Chart(ctx, {
                type: chartData.type,
                data: {
                    labels: chartData.labels,
                    datasets: datasets
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'bottom',
                            labels: {
                                boxWidth: 20,
                                padding: 10
                            }
                        }
                    },
                    scales: chartData.type === 'line' ? {
                        y: {
                            beginAtZero: true
                        }
                    } : {},
                    animations: {
                        duration: 1000,
                        easing: 'easeInOutQuart'
                    }
                }
            });
            chart.id = chartData.chartId;
            
            // Force white color for line charts after creation
            if (chartData.type === 'line') {
                setTimeout(() => {
                    chart.data.datasets.forEach(dataset => {
                        dataset.borderColor = '#ffffff';
                        dataset.pointBackgroundColor = '#ffffff';
                        dataset.pointBorderColor = '#000000';
                    });
                    chart.update('none');
                }, 100);
            }
            
            console.log('Chart created:', chart);
        } catch (error) {
            console.error('Error creating pie chart:', error);
        }
    }

    static validateChartData(chartData) {
        if (!chartData.chartId || typeof chartData.chartId !== 'string') {
            console.error('Invalid or missing chartId');
            return false;
        }
        if (!chartData.type || typeof chartData.type !== 'string') {
            console.error('Invalid or missing chart type');
            return false;
        }
        if (!Array.isArray(chartData.labels) || chartData.labels.length === 0) {
            console.error('Invalid or missing labels');
            return false;
        }
        if (!Array.isArray(chartData.datasets) || chartData.datasets.length === 0) {
            console.error('Invalid or missing datasets');
            return false;
        }
        for (const dataset of chartData.datasets) {
            if (!Array.isArray(dataset.data) || dataset.data.length === 0) {
                console.error('Invalid or missing dataset data');
                return false;
            }
            if (!Array.isArray(dataset.backgroundColor) || dataset.backgroundColor.length === 0) {
                console.error('Invalid or missing dataset backgroundColor');
                return false;
            }
        }
        return true;
    }
}
