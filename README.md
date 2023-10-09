# SGUIToolBox
**S**tarLight**G**rimmer‘s Unity UI ToolBox

星辰微光的UI工具箱

程序化UI练习项目

各脚本位于`Asset/Scripts`文件夹中，该文件夹下的test文件夹为测试代码

## SGButton

自定义的Button组件，在原有Button的基础上新增了一些内容：

- 视觉上跳过选中状态
- 鼠标点击、进入、长按时应用Scale变化（可选）
- 鼠标悬停
- 鼠标长按
- 鼠标进入/离开

## SGProgressBar

自定义的进度条组件

- 显示形式（整数，小数，百分率）
- 可设置进度条变化速率
- 可设置不同进度的颜色变化

## SGAnimText

自定义的文本动画组件，需要附加在带有`TextMeshProUGUI`的物体上

- 打字机效果
- 部分字体放大
- 字体绕中心点摆动
- 字体颜色效果
- 更多效果待开发

## TODO

- [ ] 为每个Event添加AddListener方法和RemoveListener方法
- [ ] 按钮分开长按和点击
- [ ] SGAnimText部分字体放大部分改用`textInfo.characterInfo[i].pointSize`
- [ ] 为SGAnimText添加部分字体颜色变化、部分字体动画等效果
