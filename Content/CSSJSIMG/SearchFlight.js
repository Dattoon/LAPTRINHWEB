document.querySelector('.muiten').addEventListener('click', function() {
    var content = document.querySelector('.Collapse_collapsed-content__UzsXV.Collapse_collapse__g6PnD');
    var currentHeight = content.style.height;
    content.style.height = currentHeight === '300px' ? '0px' : '300px';
  });
  