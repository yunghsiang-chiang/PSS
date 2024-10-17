$(document).ready(function () {
    console.log('SurveyCharts.js is loaded');

    // 呼叫 Web API 並繪製社群數據圓餅圖
    fetchSocialData();

    // 呼叫 Web API 並繪製年齡範圍直條圖
    fetchAgeData();

    // 呼叫 Web API 並繪製顏色選擇分布直條圖
    fetchColorDistributionData();

    // 呼叫 Web API 並繪製問卷填寫時間線性圖
    fetchSubmissionTimeData();
});

// 繪製社群數據圓餅圖
function fetchSocialData() {
    $.ajax({
        url: 'http://internal.hochi.org.tw:8082/api/activity/SocialData',
        method: 'GET',
        success: function (data) {
            const labels = data.platforms;
            const values = data.values;

            const ctx = $('#socialPieChart')[0].getContext('2d');
            new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: labels,
                    datasets: [{
                        data: values,
                        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56']
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.error('Error fetching social data:', error);
        }
    });
}

// 繪製年齡範圍直條圖
function fetchAgeData() {
    $.ajax({
        url: 'http://internal.hochi.org.tw:8082/api/activity/AgeData',
        method: 'GET',
        success: function (data) {
            const labels = data.ageRanges;
            const values = data.counts;

            const ctx = $('#ageBarChart')[0].getContext('2d');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: '人數',
                        data: values,
                        backgroundColor: '#36A2EB'
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        x: {
                            title: {
                                display: true,
                                text: '年齡範圍'
                            }
                        },
                        y: {
                            title: {
                                display: true,
                                text: '人數'
                            }
                        }
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.error('Error fetching age data:', error);
        }
    });
}

// 繪製顏色選擇分布直條圖
function fetchColorDistributionData() {
    $.ajax({
        url: 'http://internal.hochi.org.tw:8082/api/activity/ColorDistribution',
        method: 'GET',
        success: function (data) {
            const labels = ['紅', '橙', '黃', '綠', '藍', '靛', '紫'];
            const baseColors = {
                '紅': '#FF0000',
                '橙': '#FFA500',
                '黃': '#FFFF00',
                '綠': '#008000',
                '藍': '#0000FF',
                '靛': '#4B0082',
                '紫': '#800080'
            };

            const datasets = data.groups.map((group, index) => ({
                label: group, // 顏色選擇1 分組名稱 (如金光系)
                data: data.data[index].colors,
                backgroundColor: labels.map(color => adjustColorForGroup(baseColors[color], group)),
                borderColor: labels.map(color => adjustColorForGroup(baseColors[color], group)),
                borderWidth: 1
            }));

            const ctx = $('#colorDistributionBarChart')[0].getContext('2d');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels, // 顏色選擇2
                    datasets: datasets // 每個分組的數據集
                },
                options: {
                    responsive: true,
                    scales: {
                        x: {
                            title: {
                                display: true,
                                text: '顏色選擇2'
                            }
                        },
                        y: {
                            title: {
                                display: true,
                                text: '人數'
                            },
                            beginAtZero: true
                        }
                    },
                    plugins: {
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.error('Error fetching color distribution data:', error);
        }
    });
}

// 用於為每個 message_board_color1 分組調整顏色
function adjustColorForGroup(hexColor, group) {
    let adjustment = 0;

    if (group === '金光系') {
        adjustment = -50; // 調暗
    } else if (group === '銀光系') {
        adjustment = 50;  // 調亮
    }

    return adjustColorBrightness(hexColor, adjustment);
}

// 調整顏色亮度的輔助函數
function adjustColorBrightness(hex, amount) {
    let usePound = false;
    if (hex[0] === "#") {
        hex = hex.slice(1);
        usePound = true;
    }

    const num = parseInt(hex, 16);
    let r = (num >> 16) + amount;
    let g = ((num >> 8) & 0x00FF) + amount;
    let b = (num & 0x0000FF) + amount;

    r = Math.max(Math.min(255, r), 0);
    g = Math.max(Math.min(255, g), 0);
    b = Math.max(Math.min(255, b), 0);

    return (usePound ? "#" : "") + (r << 16 | g << 8 | b).toString(16).padStart(6, '0');
}





// 繪製填寫問卷的小時分布圖
function fetchSubmissionTimeData() {
    $.ajax({
        url: 'http://internal.hochi.org.tw:8082/api/activity/SubmissionTimeData',
        method: 'GET',
        success: function (data) {
            const labels = data.hours; // 0-23 小時
            const values = data.counts;

            const ctx = $('#submissionTimeLineChart')[0].getContext('2d');
            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: '提交數量',
                        data: values,
                        borderColor: '#36A2EB',
                        fill: false
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        x: {
                            title: {
                                display: true,
                                text: '小時'
                            }
                        },
                        y: {
                            title: {
                                display: true,
                                text: '提交數'
                            }
                        }
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.error('Error fetching submission time data:', error);
        }
    });
}
