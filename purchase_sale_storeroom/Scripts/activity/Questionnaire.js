$(document).ready(function () {
    $('input[name="check-in"]').change(function () {
        if ($(this).val() === '是') {
            $('#social-platforms').removeClass('hidden');
        } else {
            $('#social-platforms').addClass('hidden');
            $('select[name="social-platform"]').val('');
        }
    });

    $('input[name="message-board"]').change(function () {
        if ($(this).val() === '是') {
            $('#message-board-colors').removeClass('hidden');
        } else {
            $('#message-board-colors').addClass('hidden');
            $('select[name="message-board-color1"], select[name="message-board-color2"]').val('');
        }
    });

    $('input[name="balloon"]').change(function () {
        if ($(this).val() === '是') {
            $('#balloon-colors').removeClass('hidden');
        } else {
            $('#balloon-colors').addClass('hidden');
            $('select[name="balloon-color1"], select[name="balloon-color2"]').val('');
        }
    });

    $('select[name="color-choice"]').change(function () {
        if ($(this).val() === '其他') {
            $('#color-choice-text').removeClass('hidden');
        } else {
            $('#color-choice-text').addClass('hidden');
            $('textarea[name="color-choice-text"]').val('');
        }
    });

    $('select[name="satisfaction"]').change(function () {
        $(this).find('option[value=""]').remove(); // 移除 "請選擇" 選項
    });

    $('#refresh-btn').click(function(){
        resetForm();
    })

    $('#submit-btn').click(function (event) {
        event.preventDefault();

        // 收集表單數據
        const surveyData = {
            Gender: $('input[name="gender"]:checked').val(),
            AgeRange: $('select[name="age"]').val(),
            IsCheckIn: $('input[name="check-in"]:checked').val(),
            Platform: $('select[name="social-platform"]').val(),
            MessageBoard: $('input[name="message-board"]:checked').val(),
            MessageBoardColor1: $('select[name="message-board-color1"]').val(),
            MessageBoardColor2: $('select[name="message-board-color2"]').val(),
            Balloon: $('input[name="balloon"]:checked').val(),
            BalloonColor1: $('select[name="balloon-color1"]').val(),
            BalloonColor2: $('select[name="balloon-color2"]').val(),
            ColorChoice: $('select[name="color-choice"]').val(),
            ColorChoiceText: $('textarea[name="color-choice-text"]').val(),
            Satisfaction: $('select[name="satisfaction"]').val(),
            FeedbackText: $('textarea[name="feedback"]').val(),
            PromoterObservation: $('textarea[name="promoter-observation"]').val(),
            Others: $('textarea[name="others"]').val()
        };

        if (!surveyData.Satisfaction) {
            alert('請選擇 參與者滿意度');
            return;
        }


        // 發送 POST 請求到 Web API
        $.ajax({
            url: 'http://internal.hochi.org.tw:8082/api/activity/SubmitSurvey',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(surveyData),
            success: function (response) {
                alert('提交成功！');
                resetForm();
            },
            error: function (xhr, status, error) {
                alert('提交失敗，請重試。');
                console.error('Error:', error);
            }
        });
    });

    // 定義重製表單的函數
    function resetForm() {
        // 重製 radio 按鈕和選擇框
        $('input[name="gender"][value="男"]').prop('checked', true);
        $('select[name="age"]').val('0-10');
        $('input[name="check-in"][value="否"]').prop('checked', true);
        $('#social-platforms').addClass('hidden');
        $('select[name="social-platform"]').val('');

        $('input[name="message-board"][value="否"]').prop('checked', true);
        $('#message-board-colors').addClass('hidden');
        $('select[name="message-board-color1"], select[name="message-board-color2"]').val('');

        $('input[name="balloon"][value="否"]').prop('checked', true);
        $('#balloon-colors').addClass('hidden');
        $('select[name="balloon-color1"], select[name="balloon-color2"]').val('');

        $('select[name="color-choice"]').val('需要的');
        $('#color-choice-text').addClass('hidden');
        $('textarea[name="color-choice-text"]').val('');

        $('select[name="satisfaction"]').html('<option value="" selected>請選擇</option><option value="水下下">水下下</option><option value="水下">水下</option><option value="水上">水上</option><option value="水上上">水上上</option>');

        $('textarea[name="feedback"], textarea[name="promoter-observation"], textarea[name="others"]').val('');
    }

});