﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Questionnaire.aspx.cs" Inherits="purchase_sale_storeroom.activity.Questionnaire" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>問卷調查</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script src="../Scripts/activity/Questionnaire.js"></script>
    <style>
        .section {
            margin-bottom: 2rem;
        }

        .hidden {
            display: none;
        }

        @media (max-width: 768px) {
            .section {
                margin-bottom: 1rem;
            }

            .form-label {
                margin-bottom: 0.5rem;
            }

            .mb-3 {
                margin-bottom: 1rem;
            }
        }
    </style>
</head>
<body>
    <div class="container my-5">
        <h1 class="mb-4 text-center">2024蔬素食展攤位回饋表</h1>
        <div class="text-center mt-4">
            <button class="btn btn-primary w-100 w-md-auto" id="refresh-btn">重置問卷</button>
        </div>
        <!-- 基本資料統計 -->
        <div class="section">
            <h3>基本資料統計</h3>
            <div class="row g-3">
                <div class="col-12 col-md-6">
                    <label class="form-label">性別：</label>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="gender" value="男" checked="checked" />
                        <label class="form-check-label">男</label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="gender" value="女" />
                        <label class="form-check-label">女</label>
                    </div>
                </div>
                <div class="col-12 col-md-6">
                    <label class="form-label">年齡範圍：</label>
                    <select class="form-select" name="age">
                        <option value="0-10">0~10</option>
                        <option value="11-20">11~20</option>
                        <option value="21-30">21~30</option>
                        <option value="31-40">31~40</option>
                        <option value="41-50">41~50</option>
                        <option value="51-60">51~60</option>
                        <option value="61-70">61~70</option>
                        <option value="71-80">71~80</option>
                        <option value="80+">80以上</option>
                    </select>
                </div>
            </div>
        </div>

        <!-- 社群打卡參與狀況 -->
        <div class="section">
            <h3>社群打卡參與狀況</h3>
            <div class="row g-3">
                <div class="col-12 col-md-6">
                    <label class="form-label">追蹤打卡狀況：</label>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="check-in" value="否" checked="checked" />
                        <label class="form-check-label">否</label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="check-in" value="是" />
                        <label class="form-check-label">是</label>
                    </div>
                </div>
                <div class="col-12 col-md-6 hidden" id="social-platforms">
                    <label class="form-label">社群平台：</label>
                    <select class="form-select" name="social-platform">
                        <option value="" selected="selected">請選擇</option>
                        <option value="LINE">LINE</option>
                        <option value="FB">FB</option>
                        <option value="IG">IG</option>
                        <option value="YouTube">YouTube</option>
                    </select>
                </div>
            </div>
        </div>

        <!-- 顏色選擇統計 -->
        <div class="section">
            <h3>顏色選擇統計</h3>
            <div class="row g-3">
                <div class="col-12 col-md-6">
                    <label class="form-label">🗒️留言板：</label>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="message-board" value="否" checked="checked" />
                        <label class="form-check-label">否</label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="message-board" value="是" />
                        <label class="form-check-label">是</label>
                    </div>
                </div>
                <div class="col-12 col-md-6 hidden" id="message-board-colors">
                    <label class="form-label">顏色選擇1：</label>
                    <select class="form-select" name="message-board-color1">
                        <option value="" selected="selected">請選擇</option>
                        <option value="金光系">金光系</option>
                        <option value="銀光系">銀光系</option>
                        <option value="純光系">純光系</option>
                        <option value="無">無</option>
                    </select>
                    <label class="form-label mt-2">顏色選擇2：</label>
                    <select class="form-select" name="message-board-color2">
                        <option value="" selected="selected">請選擇</option>
                        <option value="紅">紅</option>
                        <option value="橙">橙</option>
                        <option value="黃">黃</option>
                        <option value="綠">綠</option>
                        <option value="藍">藍</option>
                        <option value="靛">靛</option>
                        <option value="紫">紫</option>
                    </select>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-12 col-md-6">
                    <label class="form-label">🎈點點貼氣球：</label>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="balloon" value="否" checked="checked" />
                        <label class="form-check-label">否</label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="balloon" value="是" />
                        <label class="form-check-label">是</label>
                    </div>
                </div>
                <div class="col-12 col-md-6 hidden" id="balloon-colors">
                    <label class="form-label">顏色選擇1：</label>
                    <select class="form-select" name="balloon-color1">
                        <option value="" selected="selected">請選擇</option>
                        <option value="金光系">金光系</option>
                        <option value="銀光系">銀光系</option>
                        <option value="純光系">純光系</option>
                        <option value="無">無</option>
                    </select>
                    <label class="form-label mt-2">顏色選擇2：</label>
                    <select class="form-select" name="balloon-color2">
                        <option value="" selected="selected">請選擇</option>
                        <option value="紅">紅</option>
                        <option value="橙">橙</option>
                        <option value="黃">黃</option>
                        <option value="綠">綠</option>
                        <option value="藍">藍</option>
                        <option value="靛">靛</option>
                        <option value="紫">紫</option>
                    </select>
                </div>
            </div>
        </div>

        <!-- 反饋與觀察 -->
        <div class="section">
            <h3>反饋與觀察</h3>
            <div class="row g-3">
                <div class="col-12 col-md-6">
                    <label class="form-label">參與者選顏色方式：</label>
                    <select class="form-select" name="color-choice">
                        <option value="需要的" selected="selected">需要的</option>
                        <option value="喜歡的">喜歡的</option>
                        <option value="隨機的">隨機的</option>
                        <option value="其他">其他</option>
                    </select>
                </div>
                <div class="col-12 col-md-6 hidden" id="color-choice-text">
                    <label class="form-label">填寫選擇顏色方式：</label>
                    <textarea class="form-control" name="color-choice-text" rows="3" placeholder="請輸入"></textarea>
                </div>
                <div class="col-12 col-md-6">
                    <label class="form-label">參與者滿意度：</label>
                    <select class="form-select" name="satisfaction">
                        <option value="" selected="selected">請選擇</option>
                        <option value="水下下">水下下</option>
                        <option value="水下">水下</option>
                        <option value="水上">水上</option>
                        <option value="水上上">水上上</option>
                    </select>
                </div>
                <div class="col-12 col-md-6">
                    <label class="form-label">參與者互動建議：</label>
                    <textarea class="form-control" name="feedback" rows="3" placeholder="請輸入建議"></textarea>
                </div>
                <div class="col-12 col-md-6">
                    <label class="form-label">推廣員的觀察：</label>
                    <textarea class="form-control" name="promoter-observation" rows="3" placeholder="請輸入觀察"></textarea>
                </div>
                <div class="col-12 col-md-6">
                    <label class="form-label">其他：</label>
                    <textarea class="form-control" name="others" rows="3" placeholder="請輸入其他意見"></textarea>
                </div>
            </div>
        </div>

        <div class="text-center mt-4">
            <button class="btn btn-primary w-100 w-md-auto" id="submit-btn">提交</button>
        </div>
    </div>
</body>
</html>
