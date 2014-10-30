function initChart(urlMapper: vdb.UrlMapper, thisTag: string, parent: string, siblings: string[], children: string[]) {

	var tagUrl = (tagName: string) => urlMapper.mapRelative("/Tag/Details/" + tagName);
	var tagLink = (tagName: string) => {
		var link = '<a href="' + tagUrl(tagName) + '">' + tagName + '</a>';
		return link;
	};

	var tagLinks = (tagList: string[]) => {

		var str = "";
		var links = _.map(tagList, item => tagLink(item));

		for (var i = 0; i < tagList.length; i += 8) {
			
			str += _.reduce<string, string>(_.take(_.rest(links, i), 8), (list, item) => list + ", " + item);

			if (i < tagList.length + 8)
				str += "<br/>";

		}

		return str;

	}

	$('#hierarchyContainer').highcharts({
		credits: { enabled: false },
		chart: {
			backgroundColor: null,
			events: {
				load: function () {

					// Draw the flow chart
					var ren = this.renderer,
						colors = Highcharts.getOptions().colors,
						downArrow = ['M', 0, 0, 'L', 0, 40, 'L', -5, 35, 'M', 0, 40, 'L', 5, 35],
						rightAndDownArrow = ['M', 0, 0, 'L', 50, 0, 'C', 70, 0, 70, 0, 70, 25,
							'L', 70, 80, 'L', 65, 75, 'M', 70, 80, 'L', 75, 75];

					var y = 10;

					if (parent) {

						ren.label("Parent tag:<br/>" + tagLink(parent), 10, y)
							.attr({
								fill: colors[0],
								stroke: 'white',
								'stroke-width': 2,
								padding: 5,
								r: 5
							})
							.css({
								color: 'white'
							})
							.add()
							.shadow(true);

						// Arrow from parent to this tag
						ren.path(downArrow)
							.translate(50, y + 60)
							.attr({
								'stroke-width': 2,
								stroke: colors[3]
							})
							.add();

						// Arrow from parent to siblings
						ren.path(rightAndDownArrow)
							.translate(100, y + 20)
							.attr({
								'stroke-width': 2,
								stroke: colors[3]
							})
							.add();

						ren.label("Siblings:<br/>" + tagLinks(siblings), 130, y + 115)
							.attr({
								fill: colors[4],
								stroke: 'white',
								'stroke-width': 2,
								padding: 5,
								r: 5
							})
							.css({
								color: 'white'
							})
							.add()
							.shadow(true);

						y += 115;

					}

					ren.label("This tag:<br />" + thisTag, 10, y)
						.attr({
							fill: colors[1],
							stroke: 'white',
							'stroke-width': 2,
							padding: 5,
							r: 5
						})
						.css({
							color: 'white'
						})
						.add()
						.shadow(true);

					if (children && children.length) {
						
						// Arrow from this to children
						ren.path(downArrow)
							.translate(50, y + 60)
							.attr({
								'stroke-width': 2,
								stroke: colors[3]
							})
							.add();

						ren.label("Children:<br/>" + tagLinks(children), 10, y + 115)
							.attr({
								fill: colors[4],
								stroke: 'white',
								'stroke-width': 2,
								padding: 5,
								r: 5
							})
							.css({
								color: 'white'
							})
							.add()
							.shadow(true);


					}

				}
			}
		},
        title: {
			text: null,
		}
	});
}