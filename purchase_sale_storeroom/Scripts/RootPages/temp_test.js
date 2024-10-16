$(document).ready(function () {

    //將問題資訊取得 並丟到 div id=example_div 
    let api_url = "http://10.10.3.75:8082/api/world/Get_distinct_question";
    var myAPI = api_url;
    $.getJSON(myAPI, {
        format: "json"
    })
        .done(function (data) {
            for (let i = 0; i <= data.length; i++) {
                var div_chlid = '<div class="row"><div class="col">' + data[i].question + '</div><div class="col"><button type="button" class="btn btn-primary straight-strip" value="' + data[i].question + '">直條圖</button></div><div class="col"><button type="button" class="btn btn-primary pie" value="' + data[i].question + '">圓餅圖</button></div></div > ';
                $("#example_div").append(div_chlid)
            }
        });

});

//將動態元件 賦予功能
$(document).bind('click', $('#example_div'), function () {
    //
    $('.btn').click(function () {
        
        let chart_type = ''; //圖表類型
        if ($(this).hasClass('btn btn-primary straight-strip')) {
            chart_type = 'bar';

        }
        if ($(this).hasClass('btn btn-primary pie')) {
            chart_type = 'pie';
        }

        get_question_answer($(this).val(), chart_type); //獲得數據

    });
    $(document).unbind();
})

//API 取得問題資料
function get_question_answer(question, charttype) {
    let api_url = "http://10.10.3.75:8082/api/world/get_question_answer?question=" + question;
    var myAPI = api_url;
    $.getJSON(myAPI, {
        format: "json"
    })
        .done(function (data) {
            //繪圖
            setting_chart(data, charttype);
            console.log(charttype + '繪圖');
        })
        .fail(function (data) {
            console.log('fail:'+data)
        });
}

//繪圖
function setting_chart(sourcedata, charttype) {
    $('.chartcontainer').empty(); //清空準備重新繪圖
    $('.chartcontainer').append('<canvas id="myChart"></canvas>'); //放置 canvas id="myChart"
    const ctx = $('#myChart'); //取得繪圖位置
    //將API data數據丟到Array,方便儲存與套模板
    datalabels = new Array();
    datacontents = new Array();
    for (var i = 0; i < sourcedata.length; i++) {
        datalabels.push(sourcedata[i].回答);
        datacontents.push(sourcedata[i].qty);
    }
    //繪圖 模板
    new Chart(ctx, {
        type: charttype, //繪圖類型
        data: {
            labels: datalabels, //X軸數據
            datasets: [{
                label: '# of Votes',
                data: datacontents, //Y軸數據
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

}
