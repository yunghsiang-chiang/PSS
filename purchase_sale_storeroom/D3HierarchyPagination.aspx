<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D3HierarchyPagination.aspx.cs" Inherits="purchase_sale_storeroom.D3HierarchyPagination" %>

<!DOCTYPE html>
<html lang="zh">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>D3.js 分頁階層圖</title>
    <script src="https://d3js.org/d3.v6.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <style>
        .node circle {
            fill: steelblue;
            stroke: #fff;
            stroke-width: 2px;
        }
        .node text {
            font-size: 12px;
            text-anchor: middle;
        }
        .link {
            fill: none;
            stroke: #ccc;
            stroke-width: 2px;
        }
        .pagination {
            margin: 10px;
            text-align: center;
        }
        .pagination button {
            padding: 5px 10px;
            margin: 5px;
            cursor: pointer;
        }
        .depth-indicator {
            display: flex;
            justify-content: center;
            margin-bottom: 10px;
        }
        .depth-indicator span {
            display: inline-block;
            padding: 5px 15px;
            margin: 0 5px;
            border-radius: 5px;
            color: white;
            font-weight: bold;
        }
        @media (max-width: 768px) {
            svg {
                width: 100% !important;
                height: auto !important;
            }
        }
    </style>
</head>
<body class="container">
    <div class="depth-indicator" id="depthIndicator"></div>
    <div class="pagination" id="pagination"></div>
    <svg width="100%" height="600"></svg>
    <script>
        const nodes = [{ 'node_id': 2, 'parent_id': 1, 'data': '陳曉利' },
            { 'node_id': 3, 'parent_id': 1, 'data': '陳君明' },
            { 'node_id': 5, 'parent_id': 4, 'data': '洪怡芬' },
            { 'node_id': 6, 'parent_id': 4, 'data': '曾國華' },
            { 'node_id': 7, 'parent_id': 4, 'data': '范䕒云' },
            { 'node_id': 9, 'parent_id': 8, 'data': '林寶惠' },
            { 'node_id': 10, 'parent_id': 8, 'data': '陳秀珠' },
            { 'node_id': 11, 'parent_id': 8, 'data': '戴珠誼' },
            { 'node_id': 13, 'parent_id': 12, 'data': '王玉梅' },
            { 'node_id': 14, 'parent_id': 12, 'data': '許秋玉' },
            { 'node_id': 16, 'parent_id': 15, 'data': '石美月' },
            { 'node_id': 18, 'parent_id': 17, 'data': '唐蓓玲' },
            { 'node_id': 19, 'parent_id': 17, 'data': '林嘉鳳' },
            { 'node_id': 21, 'parent_id': 20, 'data': '趙秀珠' },
            { 'node_id': 22, 'parent_id': 20, 'data': '黃淑玲' },
            { 'node_id': 19, 'parent_id': 23, 'data': '林嘉鳳' },
            { 'node_id': 25, 'parent_id': 24, 'data': '張瓊月' },
            { 'node_id': 26, 'parent_id': 24, 'data': '楊智驊' },
            { 'node_id': 23, 'parent_id': 27, 'data': '林秋姍' },
            { 'node_id': 28, 'parent_id': 27, 'data': '黃秀琴' },
            { 'node_id': 29, 'parent_id': 28, 'data': '沈修束' },
            { 'node_id': 30, 'parent_id': 28, 'data': '郭舒文' },
            { 'node_id': 4, 'parent_id': 31, 'data': '程淑娟' },
            { 'node_id': 6, 'parent_id': 4, 'data': '曾國華' },
            { 'node_id': 5, 'parent_id': 4, 'data': '洪怡芬' },
            { 'node_id': 14, 'parent_id': 12, 'data': '許秋玉' },
            { 'node_id': 13, 'parent_id': 12, 'data': '王玉梅' },
            { 'node_id': 18, 'parent_id': 23, 'data': '唐蓓玲' },
            { 'node_id': 19, 'parent_id': 23, 'data': '林嘉鳳' },
            { 'node_id': 15, 'parent_id': 32, 'data': '蔡玲雪' },
            { 'node_id': 34, 'parent_id': 33, 'data': '顏秋羚' },
            { 'node_id': 35, 'parent_id': 33, 'data': '張琬珮' },
            { 'node_id': 36, 'parent_id': 33, 'data': '蕭菁根' },
            { 'node_id': 37, 'parent_id': 33, 'data': '徐秀芳' },
            { 'node_id': 38, 'parent_id': 33, 'data': '陳美娟' },
            { 'node_id': 39, 'parent_id': 33, 'data': '劉淑如' },
            { 'node_id': 40, 'parent_id': 33, 'data': '蔣伯澤' },
            { 'node_id': 42, 'parent_id': 41, 'data': '邱玫瑛' },
            { 'node_id': 43, 'parent_id': 41, 'data': '蘇麗真' },
            { 'node_id': 44, 'parent_id': 41, 'data': '秦麗花' },
            { 'node_id': 1, 'parent_id': 45, 'data': '游慧眞' },
            { 'node_id': 46, 'parent_id': 45, 'data': '陳瑞愛' },
            { 'node_id': 3, 'parent_id': 45, 'data': '陳君明' },
            { 'node_id': 48, 'parent_id': 47, 'data': '高夏子' },
            { 'node_id': 49, 'parent_id': 47, 'data': '張馥媛' },
            { 'node_id': 51, 'parent_id': 50, 'data': '張鈴雪' },
            { 'node_id': 52, 'parent_id': 50, 'data': '吳春燕' },
            { 'node_id': 54, 'parent_id': 53, 'data': '林桂枝' },
            { 'node_id': 55, 'parent_id': 53, 'data': '廖家綺' },
            { 'node_id': 56, 'parent_id': 53, 'data': '林惠欣' },
            { 'node_id': 41, 'parent_id': 53, 'data': '邱玫瑾' },
            { 'node_id': 42, 'parent_id': 53, 'data': '邱玫瑛' },
            { 'node_id': 58, 'parent_id': 57, 'data': '劉聰賢' },
            { 'node_id': 59, 'parent_id': 57, 'data': '簡均璇' },
            { 'node_id': 61, 'parent_id': 60, 'data': '陳麗美' },
            { 'node_id': 62, 'parent_id': 60, 'data': '陳麗真' },
            { 'node_id': 64, 'parent_id': 63, 'data': '張丹丹' },
            { 'node_id': 65, 'parent_id': 63, 'data': '張加佳' },
            { 'node_id': 66, 'parent_id': 58, 'data': '游釧媌' },
            { 'node_id': 67, 'parent_id': 58, 'data': '劉鳳英' },
            { 'node_id': 68, 'parent_id': 56, 'data': '翁君玉' },
            { 'node_id': 69, 'parent_id': 56, 'data': '田慧娟' },
            { 'node_id': 40, 'parent_id': 35, 'data': '蔣伯澤' },
            { 'node_id': 70, 'parent_id': 69, 'data': '高慧娟' },
            { 'node_id': 71, 'parent_id': 69, 'data': '邱麗蒖' },
            { 'node_id': 72, 'parent_id': 69, 'data': '蘇麗君' },
            { 'node_id': 73, 'parent_id': 34, 'data': '曹昌崙' },
            { 'node_id': 74, 'parent_id': 34, 'data': '李綉鳳' },
            { 'node_id': 75, 'parent_id': 34, 'data': '胡秀蓉' },
            { 'node_id': 76, 'parent_id': 34, 'data': '莊慈珍' },
            { 'node_id': 77, 'parent_id': 34, 'data': '羅文酉' },
            { 'node_id': 78, 'parent_id': 34, 'data': '柯玉齡' },
            { 'node_id': 79, 'parent_id': 34, 'data': '王晶縈' },
            { 'node_id': 80, 'parent_id': 34, 'data': '張秀枝' },
            { 'node_id': 81, 'parent_id': 34, 'data': '黃秋末' },
            { 'node_id': 82, 'parent_id': 74, 'data': '張瓊芬' },
            { 'node_id': 83, 'parent_id': 74, 'data': '吳宜樺' },
            { 'node_id': 84, 'parent_id': 74, 'data': '李麗敏' },
            { 'node_id': 85, 'parent_id': 75, 'data': '陳美惠' },
            { 'node_id': 86, 'parent_id': 75, 'data': '李香蓮' },
            { 'node_id': 87, 'parent_id': 76, 'data': '吳幸珍' },
            { 'node_id': 88, 'parent_id': 76, 'data': '魏瑞萍' },
            { 'node_id': 89, 'parent_id': 77, 'data': '謝岳淋' },
            { 'node_id': 90, 'parent_id': 77, 'data': '林瓊瑤' },
            { 'node_id': 91, 'parent_id': 77, 'data': '李輝賢' },
            { 'node_id': 92, 'parent_id': 78, 'data': '蕭淑妃' },
            { 'node_id': 93, 'parent_id': 79, 'data': '林靜蘭' },
            { 'node_id': 94, 'parent_id': 81, 'data': '劉玉卿' },
            { 'node_id': 95, 'parent_id': 81, 'data': '許月桂' },
            { 'node_id': 60, 'parent_id': 37, 'data': '蔡蕎蔓' },
            { 'node_id': 96, 'parent_id': 71, 'data': '歐貞均' },
            { 'node_id': 97, 'parent_id': 71, 'data': '劉慧蔆' },
            { 'node_id': 98, 'parent_id': 40, 'data': '陳英錦' },
            { 'node_id': 63, 'parent_id': 40, 'data': '李幸娟' },
            { 'node_id': 99, 'parent_id': 98, 'data': '鄭皓中' },
            { 'node_id': 100, 'parent_id': 98, 'data': '王慧玲' },
            { 'node_id': 39, 'parent_id': 38, 'data': '劉淑如' },
            { 'node_id': 101, 'parent_id': 38, 'data': '游璧瑜' },
            { 'node_id': 102, 'parent_id': 38, 'data': '鄭祐欣 ' },
            { 'node_id': 103, 'parent_id': 38, 'data': '張瀞文' },
            { 'node_id': 54, 'parent_id': 53, 'data': '林桂枝' },
            { 'node_id': 55, 'parent_id': 53, 'data': '廖家綺' },
            { 'node_id': 56, 'parent_id': 53, 'data': '林惠欣' },
            { 'node_id': 41, 'parent_id': 53, 'data': '邱玫瑾' },
            { 'node_id': 104, 'parent_id': 7, 'data': '謝亞軒' },
            { 'node_id': 105, 'parent_id': 7, 'data': '王君如' },
            { 'node_id': 106, 'parent_id': 43, 'data': '陳淑燕' },
            { 'node_id': 107, 'parent_id': 43, 'data': '林恬毓' },
            { 'node_id': 109, 'parent_id': 108, 'data': '洪妙如' },
            { 'node_id': 110, 'parent_id': 108, 'data': '蕭美琪' },
            { 'node_id': 111, 'parent_id': 108, 'data': '蘇淑美' },
            { 'node_id': 113, 'parent_id': 112, 'data': '薛明早' },
            { 'node_id': 114, 'parent_id': 112, 'data': '薛美淑' },
            { 'node_id': 115, 'parent_id': 109, 'data': '蔡帛庭' },
            { 'node_id': 116, 'parent_id': 109, 'data': '張雅涵' },
            { 'node_id': 117, 'parent_id': 44, 'data': '楊淑惠' },
            { 'node_id': 118, 'parent_id': 42, 'data': '林千沛' },
            { 'node_id': 116, 'parent_id': 111, 'data': '張雅涵' },
            { 'node_id': 119, 'parent_id': 111, 'data': '郭姿伶' },
            { 'node_id': 121, 'parent_id': 120, 'data': '蔡碧珠' },
            { 'node_id': 122, 'parent_id': 120, 'data': '莊勝雄' },
            { 'node_id': 123, 'parent_id': 21, 'data': '張惠敏' },
            { 'node_id': 125, 'parent_id': 124, 'data': '王素霞' },
            { 'node_id': 126, 'parent_id': 124, 'data': '楊春茶' },
            { 'node_id': 128, 'parent_id': 127, 'data': '戴秀菊' },
            { 'node_id': 129, 'parent_id': 127, 'data': '呂祐旭' },
            { 'node_id': 131, 'parent_id': 130, 'data': '陳瓊嬅' },
            { 'node_id': 132, 'parent_id': 130, 'data': '王佩綺' },
            { 'node_id': 133, 'parent_id': 55, 'data': '郭乃菁' },
            { 'node_id': 134, 'parent_id': 55, 'data': '張雅惠' },
            { 'node_id': 136, 'parent_id': 135, 'data': '林岱霓' },
            { 'node_id': 137, 'parent_id': 135, 'data': '邱姿華' },
            { 'node_id': 133, 'parent_id': 138, 'data': '郭乃菁' },
            { 'node_id': 139, 'parent_id': 138, 'data': '方瓊凰' },
            { 'node_id': 140, 'parent_id': 5, 'data': '王佩珊' },
            { 'node_id': 141, 'parent_id': 5, 'data': '趙秀卿' },
            { 'node_id': 143, 'parent_id': 142, 'data': '蕭美慧' },
            { 'node_id': 144, 'parent_id': 142, 'data': '馮綉雯' },
            { 'node_id': 146, 'parent_id': 145, 'data': '陳怡任' },
            { 'node_id': 147, 'parent_id': 145, 'data': '賴菊蘭' },
            { 'node_id': 148, 'parent_id': 46, 'data': '巫裕棠' },
            { 'node_id': 149, 'parent_id': 46, 'data': '彭錦妹' },
            { 'node_id': 151, 'parent_id': 150, 'data': '簡妙如' },
            { 'node_id': 145, 'parent_id': 152, 'data': '顏秋絨' },
            { 'node_id': 150, 'parent_id': 152, 'data': '郭淑女' },
            { 'node_id': 153, 'parent_id': 152, 'data': '凃韋君' },
            { 'node_id': 154, 'parent_id': 126, 'data': '林聰耀' },
            { 'node_id': 155, 'parent_id': 126, 'data': '許碧玉' },
            { 'node_id': 145, 'parent_id': 156, 'data': '顏秋絨' },
            { 'node_id': 150, 'parent_id': 156, 'data': '郭淑女' },
            { 'node_id': 153, 'parent_id': 156, 'data': '凃韋君' },
            { 'node_id': 157, 'parent_id': 110, 'data': '張雅函' },
            { 'node_id': 115, 'parent_id': 110, 'data': '蔡帛庭' },
            { 'node_id': 151, 'parent_id': 150, 'data': '簡妙如' },
            { 'node_id': 9, 'parent_id': 10, 'data': '林寶惠' },
            { 'node_id': 158, 'parent_id': 129, 'data': '呂欣翰' },
            { 'node_id': 159, 'parent_id': 129, 'data': '呂明翰' },
            { 'node_id': 160, 'parent_id': 144, 'data': '方朱衣絹' },
            { 'node_id': 161, 'parent_id': 144, 'data': '朱麗琴' },
            { 'node_id': 163, 'parent_id': 162, 'data': '簡素珠' },
            { 'node_id': 164, 'parent_id': 162, 'data': '張采蘋' },
            { 'node_id': 165, 'parent_id': 16, 'data': '莊雅惠' },
            { 'node_id': 167, 'parent_id': 166, 'data': '張潔心' },
            { 'node_id': 168, 'parent_id': 166, 'data': '莊蓮芝' },
            { 'node_id': 169, 'parent_id': 166, 'data': '袁蕙琳' },
            { 'node_id': 171, 'parent_id': 170, 'data': '葉慧貞' },
            { 'node_id': 172, 'parent_id': 170, 'data': '吳秋薇' },
            { 'node_id': 173, 'parent_id': 170, 'data': '林淑慧' },
            { 'node_id': 175, 'parent_id': 174, 'data': '張翠蓮' },
            { 'node_id': 176, 'parent_id': 174, 'data': '黃秀寬' },
            { 'node_id': 178, 'parent_id': 177, 'data': '虞曉玫' },
            { 'node_id': 179, 'parent_id': 177, 'data': '楊毓萱' },
            { 'node_id': 181, 'parent_id': 180, 'data': '李秉芳' },
            { 'node_id': 182, 'parent_id': 180, 'data': '李羽羚' },
            { 'node_id': 21, 'parent_id': 183, 'data': '趙秀珠' },
            { 'node_id': 112, 'parent_id': 183, 'data': '陳昱妏' },
            { 'node_id': 185, 'parent_id': 184, 'data': '王倩婉' },
            { 'node_id': 186, 'parent_id': 184, 'data': '林專絲' },
            { 'node_id': 8, 'parent_id': null, 'data': '游嵐淇' },
            { 'node_id': 12, 'parent_id': null, 'data': '郭玫玲' },
            { 'node_id': 17, 'parent_id': null, 'data': '黃賽琳' },
            { 'node_id': 20, 'parent_id': null, 'data': '張淑滿' },
            { 'node_id': 24, 'parent_id': null, 'data': '王淑寬' },
            { 'node_id': 27, 'parent_id': null, 'data': '林淑嫈' },
            { 'node_id': 31, 'parent_id': null, 'data': '黃秀華' },
            { 'node_id': 32, 'parent_id': null, 'data': '王淑卿' },
            { 'node_id': 33, 'parent_id': null, 'data': '嚴淑芳' },
            { 'node_id': 45, 'parent_id': null, 'data': '陳水盛' },
            { 'node_id': 47, 'parent_id': null, 'data': '張紀萍' },
            { 'node_id': 50, 'parent_id': null, 'data': '郭嬋娟' },
            { 'node_id': 53, 'parent_id': null, 'data': '黃素珍' },
            { 'node_id': 57, 'parent_id': null, 'data': '孫郁芬' },
            { 'node_id': 108, 'parent_id': null, 'data': '王惠怡' },
            { 'node_id': 120, 'parent_id': null, 'data': '劉禹彤' },
            { 'node_id': 124, 'parent_id': null, 'data': '陳星平' },
            { 'node_id': 127, 'parent_id': null, 'data': '葉怡君' },
            { 'node_id': 130, 'parent_id': null, 'data': '陳麗珠' },
            { 'node_id': 135, 'parent_id': null, 'data': '劉玉惠' },
            { 'node_id': 138, 'parent_id': null, 'data': '方秋雲' },
            { 'node_id': 152, 'parent_id': null, 'data': '昱彣彣' },
            { 'node_id': 156, 'parent_id': null, 'data': '陳昱彣' },
            { 'node_id': 162, 'parent_id': null, 'data': '余秀梅' },
            { 'node_id': 166, 'parent_id': null, 'data': '鄭錦媞' },
            { 'node_id': 170, 'parent_id': null, 'data': '王嵩逸' },
            { 'node_id': 174, 'parent_id': null, 'data': '林小麗' },
            { 'node_id': 177, 'parent_id': null, 'data': '江秀蘭' },
            { 'node_id': 180, 'parent_id': null, 'data': '魏秀芬' },
            { 'node_id': 183, 'parent_id': null, 'data': '李世玉' },
            { 'node_id': 184, 'parent_id': null, 'data': '游麗芬' }]; // 範例資料，請替換為你的完整 JSON

        const nodeMap = new Map(nodes.map(d => [d.node_id, d]));
        let rootNodes = nodes.filter(d => d.parent_id === null);

        function drawTree(rootId) {
            let rootNode = nodeMap.get(rootId);
            if (!rootNode) return;
            d3.select("svg").selectAll("g").remove();
            const width = document.querySelector("svg").clientWidth;
            const height = 600;
            const svg = d3.select("svg"),
                g = svg.append("g").attr("transform", "translate(40,40)");

            let treeData = buildTree(rootId);
            if (!treeData) return;
            const treeLayout = d3.tree().size([width - 100, height - 200]);
            const rootHierarchy = d3.hierarchy(treeData, d => d.children || []);
            treeLayout(rootHierarchy);

            g.selectAll(".link")
                .data(rootHierarchy.links())
                .enter().append("path")
                .attr("class", "link")
                .attr("d", d3.linkVertical().x(d => d.x).y(d => d.y));

            const node = g.selectAll(".node")
                .data(rootHierarchy.descendants())
                .enter().append("g")
                .attr("class", "node")
                .attr("transform", d => `translate(${d.x},${d.y})`);

            node.append("circle").attr("r", 6);
            node.append("text").attr("dy", "-10").text(d => d.data.data);
        }

        function buildTree(rootId) {
            let rootNode = nodeMap.get(rootId);
            if (!rootNode) return null;
            function recurse(node, depth = 0) {
                let children = nodes.filter(d => d.parent_id === node.node_id);
                return { ...node, depth, children: children.length > 0 ? children.map(child => recurse(child, depth + 1)) : [] };
            }
            return recurse(rootNode);
        }

        function setupPagination() {
            $("#pagination").empty();
            $("#depthIndicator").empty();
            rootNodes.forEach((root) => {
                $("#pagination").append(`<button class='btn btn-primary m-1' onclick='drawTree(${root.node_id})'>${root.data}</button>`);
            });
            drawTree(rootNodes[0]?.node_id);
        }

        setupPagination();
    </script>
</body>
</html>
