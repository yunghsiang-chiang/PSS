$(document).ready(function () {
    $('#submit-btn').click(function () {
        const userId = new URLSearchParams(window.location.search).get('UserID');
        const colorGroup = $('#colorGroup').val();
        const color = $('#color').val();
        const isAttendance = $('input[name="IsAttendance"]:checked').val() === '是';

        const formData = {
            userId: userId,
            colorGroup: colorGroup,
            color: color,
            IsAttendance: isAttendance,
            attendanceDate: new Date().toISOString().slice(0, 10) // 格式化為 YYYY-MM-DD
        };

        $.ajax({
            url: 'http://internal.hochi.org.tw:8082//api/heip/SubmitExhibition',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                // 成功提交後的處理
                alert('提交成功！');
            },
            error: function (xhr, status, error) {
                if (xhr.status === 409) {
                    // 當回應是 "This user has already submitted the survey today."
                    alert('此用戶今天已提交過問卷！');
                } else {
                    // 其他錯誤情況
                    alert('提交失敗，請重試。');
                    console.error('Error:', error);
                }
            }
        });
    });
});
