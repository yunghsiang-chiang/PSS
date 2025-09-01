// /Scripts/mentor.js
(function ($) {
    $(function () {
        var $q = $('#q');
        var $btnSearch = $('#btnSearch');
        var $resultList = $('#resultList');
        var $tree = $('#tree');

        var addRelModal = new bootstrap.Modal(document.getElementById('addRelModal'));
        var $selectedMemberDisplay = $('#selectedMemberDisplay');
        var $mentorQuery = $('#mentorQuery');
        var $btnSearchMentor = $('#btnSearchMentor');
        var $mentorResult = $('#mentorResult');
        var $mentorType = $('#mentorType');
        var $mentorRole = $('#mentorRole');
        var $startDate = $('#startDate');
        var $endDate = $('#endDate');
        var $primaryYN = $('#primaryYN');
        var $remark = $('#remark');
        var $btnSubmitRel = $('#btnSubmitRel');

        var selectedMember = null;
        var selectedMentor = null;

        // 以當前頁面為基底；若是 /MentorRegister（Friendly URL）就自動補 .aspx
        var apiBase = (function () {
            var p = window.location.pathname;
            if (!/\.aspx$/i.test(p)) p = p.replace(/\/$/, '') + '.aspx';
            return p;
        })();

        function post(method, data) {
            return $.ajax({
                url: apiBase + '/' + method,
                type: 'POST',
                data: JSON.stringify(data || {}),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                xhrFields: { withCredentials: true } // 帶上 cookies（若站台需要驗證）
            }).then(function (resp) { return resp.d; });
        }

        function showAjaxError(xhr) {
            var msg = (xhr && xhr.responseJSON && xhr.responseJSON.Message)
                || (xhr && xhr.responseText) || '發生錯誤';
            alert(msg);
            console.error('AJAX Error:', xhr);
        }

        function renderList(items) {
            if (!items || items.length === 0) {
                $resultList.html('<div class="text-muted">查無資料</div>');
                return;
            }
            var html = items.map(function (it) {
                return '<div class="d-flex align-items-center justify-content-between border rounded p-2 mb-2">'
                    + '<div>' + it.DisplayText + '</div>'
                    + '<div>'
                    + '  <button class="btn btn-sm btn-outline-primary btn-add" data-id="' + it.Id + '" data-name="' + it.DisplayText + '">＋ 新增護持關係</button>'
                    + '  <button class="btn btn-sm btn-link btn-tree" data-id="' + it.Id + '">查看樹</button>'
                    + '</div>'
                    + '</div>';
            }).join('');
            $resultList.html(html);

            $resultList.find('.btn-add').on('click', function () {
                selectedMember = { id: parseInt($(this).data('id')), displayText: $(this).data('name') };
                $selectedMemberDisplay.val(selectedMember.displayText);
                selectedMentor = null;
                $mentorQuery.val('');
                $mentorResult.html('請先搜尋導師。');
                $remark.val('');
                $primaryYN.prop('checked', false);
                $startDate.val(new Date().toISOString().substring(0, 10));
                $endDate.val('');

                $.when(post('GetTypes'), post('GetRoles')).done(function (types, roles) {
                    var t = '<option value="">（無）</option>' + types.map(function (x) { return '<option value="' + x.hid + '">' + x.hmentorTypeName + '</option>'; }).join('');
                    var r = '<option value="">（無）</option>' + roles.map(function (x) { return '<option value="' + x.hid + '">' + x.hmentorRoleName + '</option>'; }).join('');
                    $mentorType.html(t);
                    $mentorRole.html(r);
                    addRelModal.show();
                }).fail(showAjaxError);
            });

            $resultList.find('.btn-tree').on('click', function () {
                loadTree(parseInt($(this).data('id')));
            });
        }

        function renderMentorList(items) {
            if (!items || items.length === 0) {
                $mentorResult.html('<div class="text-muted">查無導師</div>');
                return;
            }
            var html = items.map(function (it) {
                return '<div class="border rounded p-2 mb-2 d-flex justify-content-between align-items-center">'
                    + '<div>' + it.DisplayText + '</div>'
                    + '<button class="btn btn-sm btn-outline-success btn-pick" data-id="' + it.Id + '" data-name="' + it.DisplayText + '">選擇</button>'
                    + '</div>';
            }).join('');
            $mentorResult.html(html);

            $mentorResult.find('.btn-pick').on('click', function () {
                selectedMentor = { id: parseInt($(this).data('id')), displayText: $(this).data('name') };
                $mentorResult.html('<div class="text-success">已選擇：' + selectedMentor.displayText + '</div>');
            });
        }

        function renderTree(node) {
            if (!node) {
                $tree.html('<div class="text-muted">尚無樹狀資料</div>');
                return;
            }
            function renderNode(n) {
                var meta = n.Meta ? '<span class="badge text-bg-light ms-2">' + n.Meta + '</span>' : '';
                if (!n.Children || n.Children.length === 0) {
                    return '<li><span class="node">' + n.Label + meta + '</span></li>';
                }
                return '<li>'
                    + '<span class="node">' + n.Label + meta + '</span>'
                    + '<ul>' + n.Children.map(renderNode).join('') + '</ul>'
                    + '</li>';
            }
            $tree.html('<ul class="tree-root">' + renderNode(node) + '</ul>');
        }

        // 事件：搜尋學員
        $btnSearch.on('click', function () {
            var q = ($q.val() || '').trim();
            post('SearchMembers', { q: q }).then(renderList).fail(showAjaxError);
        });

        // 事件：在 Modal 中搜尋導師
        $btnSearchMentor.on('click', function () {
            var q = ($mentorQuery.val() || '').trim();
            if (!q) { $mentorResult.html('<div class="text-muted">請輸入關鍵字</div>'); return; }
            post('SearchMembers', { q: q }).then(function (data) {
                if (selectedMember) data = data.filter(function (x) { return x.Id !== selectedMember.id; });
                renderMentorList(data);
            }).fail(showAjaxError);
        });

        // 事件：送出新增護持關係
        $btnSubmitRel.on('click', function () {
            if (!selectedMember) { alert('請先選擇學員'); return; }
            if (!selectedMentor) { alert('請先選擇導師'); return; }

            var payload = {
                hMemberID: selectedMember.id,
                hMentorMemberID: selectedMentor.id,
                hMentorTypeID: $mentorType.val() ? parseInt($mentorType.val()) : null,
                hMentorRoleID: $mentorRole.val() ? parseInt($mentorRole.val()) : null,
                hStartDate: $startDate.val() || null,
                hEndDate: $endDate.val() || null,
                hPrimaryYN: $primaryYN.is(':checked'),
                hRemark: $remark.val() || null
            };

            post('CreateRelationship', payload).then(function (res) {
                if (res && res.ok) {
                    alert('新增成功');
                    addRelModal.hide();
                    loadTree(selectedMember.id);
                }
            }).fail(showAjaxError);
        });

        function loadTree(id) {
            post('GetTree', { memberId: id }).then(renderTree).fail(showAjaxError);
        }
    });
})(jQuery);
