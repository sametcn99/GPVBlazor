export class PieChart {
    static createPieChart(chartModelJSON) {
        try {
            const chartData = JSON.parse(chartModelJSON);

            if (!this.validateChartData(chartData)) {
                console.error('Invalid chart data:', chartData);
                return;
            }

            const ctx = document.getElementById(chartData.chartId);
            if (!ctx) {
                console.error(`Element with id ${chartData.chartId} not found.`);
                return;
            }

            const chart = new Chart(ctx, {
                type: chartData.type,
                data: {
                    labels: chartData.labels,
                    datasets: chartData.datasets.map(dataset => ({
                        label: chartData.label,
                        data: dataset.data,
                        backgroundColor: dataset.backgroundColor,
                        borderWidth: 1,
                        hoverOffset: 4
                    }))
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'right',
                            labels: {
                                boxWidth: 20,
                                padding: 10
                            }
                        }
                    },
                    animations: {
                        tension: {
                            duration: 1000,
                            easing: 'linear',
                            from: 1,
                            to: 0,
                            loop: true
                        }
                    }
                }
            });
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
