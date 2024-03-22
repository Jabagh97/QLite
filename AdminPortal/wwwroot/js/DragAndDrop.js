
function dragAndDrop(className) {

   

    const targets = []

    const cleanXLine = () => {
        const guideLineX = document.querySelector('.guide-line-x')
        guideLineX.style.left = 0
        guideLineX.style.top = 0
        guideLineX.style.width = 0
        guideLineX.style.height = 0
    }

    const cleanYLine = () => {
        const guideLineY = document.querySelector('.guide-line-y')
        guideLineY.style.left = 0
        guideLineY.style.top = 0
        guideLineY.style.width = 0
        guideLineY.style.height = 0
    }

    const resetGuideLine = () => {
        cleanXLine()
        cleanYLine()
    }

    const handleStart = (event) => {
        // get all interactive elements
        targets.length = 0

        const elements = document.querySelectorAll('.resize-drag')
        if (elements.length === 1) {
            singleElem = true;
        }

        elements.forEach((element) => {
            const rect = element.getBoundingClientRect()
            const { x, y, width, height } = rect

            if (element === event.target) return

            const actualX = x + window.scrollX
            const actualY = y + window.scrollY

            const range = 8

            targets.push({
                x: actualX,
                range,
                rect,
                element,
            })

            targets.push({
                x: actualX + width,
                range,
                rect,
                element,
            })

            targets.push({
                x: actualX + width / 2,
                range,
                rect,
                element,
            })

            targets.push({
                y: actualY,
                range,
                rect,
                element,
            })

            targets.push({
                y: actualY + height,
                range,
                rect,
                element,
            })

            targets.push({
                y: actualY + height / 2,
                range,
                rect,
                element,
            })
        })
    }


    const drawGuideLine = (event) => {
        const inRange = event.modifiers.length ? event.modifiers[0]?.inRange : false

        if (inRange) {
            const guideLineX = document.querySelector('.guide-line-x')
            const guideLineY = document.querySelector('.guide-line-y')
            const {
                x: xModifier,
                y: yModifier,
                rect,
            } = event.modifiers[0].target.source

            const canvasRect = document.getElementById('canvas-container').getBoundingClientRect()
            const { x, y } = event.target.getBoundingClientRect()

            if (xModifier) {
                guideLineX.style.left = `${xModifier - canvasRect.x + 10}px`
                guideLineX.style.top = `${Math.min(rect.y, y) - canvasRect.y + 10}px`
                guideLineX.style.width = '5px'
                guideLineX.style.height = `${Math.abs(rect.y - y)}px`
                cleanYLine()
            }
            if (yModifier) {
                guideLineY.style.left = `${Math.min(rect.x, x) - canvasRect.x + 10}px`
                guideLineY.style.top = `${yModifier - canvasRect.y - window.scrollY + 10}px`
                guideLineY.style.width = `${Math.abs(rect.x - x)}px`
                guideLineY.style.height = '5px'
                cleanXLine()
            }
        } else {
            resetGuideLine()
        }
    }

    var target

    function dragMoveListener(event) {
       // drawGuideLine(event)
        target = event.target

        // keep the dragged position in the data-x/data-y attributes
        var x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx
        var y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy

        // translate the element
        target.style.transform = 'translate(' + x + 'px, ' + y + 'px)'


        // update the posiion attributes
        target.setAttribute('data-x', x)
        target.setAttribute('data-y', y)

        // Update the position in desPageData.Comps
        var compId = target.getAttribute('data-comp-id');
        if (compId) {
            var compIndex = desPageData.Comps.findIndex(comp => comp.Id === compId);
            if (compIndex !== -1) {
                desPageData.Comps[compIndex].PosX = x + 'px';
                desPageData.Comps[compIndex].PosY = y + 'px';
                // Populate input fields if the component exists
                document.getElementById('posXInput').value = x + 'px';
                document.getElementById('posYInput').value = y + 'px';
                document.getElementById('widthInput').value = desPageData.Comps[compIndex].Width;
                document.getElementById('heightInput').value = desPageData.Comps[compIndex].Height;
                document.getElementById('selectedComp').value = desPageData.Comps[compIndex].ButtonText;
            }
        }

        x = String(x)
        y = String(y)




        targetId = target.getAttribute('id');
    }



    // this function is used later in the resizing and gesture demos
    window.dragMoveListener = dragMoveListener


    //Resize and drag
    var classCSS = '.' + className
    document.addEventListener('mouseover', function (event) {
        var element = event.target;
        if (element.classList.contains('drag-icon')) {

            interact(classCSS)

                .resizable({
                    // resize from all edges and corners
                    edges: { right: true, bottom: true },
                    listeners: {
                        move(event) {

                            var target = event.target
                            var x = (parseFloat(target.getAttribute('data-x')))
                            var y = (parseFloat(target.getAttribute('data-y')))
                            var targetId = target.getAttribute('id');
                            // update the element's style
                            target.style.width = event.rect.width + 'px'
                            target.style.height = event.rect.height + 'px'

                            //console.log(target)
                            // translate when resizing from top or left edges
                            x += event.deltaRect.left
                            y += event.deltaRect.top

                            target.style.transform = 'translate(' + x + 'px,' + y + 'px)'

                            // Update the position in desPageData.Comps
                            var compId = target.getAttribute('data-comp-id');
                            if (compId) {
                                var compIndex = desPageData.Comps.findIndex(comp => comp.Id === compId);
                                if (compIndex !== -1) {
                                    desPageData.Comps[compIndex].Width = event.rect.width + 'px'
                                    desPageData.Comps[compIndex].Height = event.rect.height + 'px'

                                    document.getElementById('widthInput').value = desPageData.Comps[compIndex].Width;
                                    document.getElementById('heightInput').value = desPageData.Comps[compIndex].Height;
                                    document.getElementById('selectedComp').value = desPageData.Comps[compIndex].ButtonText;
                                }
                            }

                            target.setAttribute('data-x', x)
                            target.setAttribute('data-y', y)
                            x = String(x)
                            y = String(y)


                           

                        }
                    },
                    modifiers: [
                        // keep the edges inside the parent
                        interact.modifiers.restrictEdges({
                            outer: 'parent',
                        }),

                        // minimum size
                        interact.modifiers.restrictSize({
                            min: { width: 50, height: 50 }
                        })
                    ],


                })
                .draggable({

                    listeners: {
                        start: handleStart,
                        move: window.dragMoveListener,
                        // call this function on every dragend event
                        end(event) {
                            resetGuideLine()
                            console.log("drag ended")
                        }
                    },
                    modifiers: [
                        interact.modifiers.snap({
                            targets: targets,
                            //range: Infinity,
                            relativePoints: [
                                { x: 0, y: 0 }, // snap relative to the element's top-left,
                                { x: 0.5, y: 0.5 }, // to the center
                                { x: 1, y: 1 }, // and to the bottom-right
                            ],
                        }),
                        interact.modifiers.restrict({
                            restriction: 'parent',
                            elementRect: { top: 0, left: 0, bottom: 1, right: 1 },
                            endOnly: true
                        })
                    ],
                    inertia: true,
                    // enable autoScroll
                    autoScroll: true,
                })
                //Enable/Disable Drop zones 
                .dropzone({
                    accept: classCSS,
                    overlap: 0.1,
                    ondropactivate: function (event) {
                        // add active dropzone feedback

                    },
                    ondragenter: function (event) {

                        var dropzoneElement = event.target

                        // feedback the possibility of a drop

                    },
                    ondragleave: function (event) {
                        // remove the drop feedback style

                    },

                    ondrop: function (event) {
                        //feedBack after drop is done 

                    },
                    ondropdeactivate: function (event) {

                        // remove active dropzone feedback
                        var draggableElement = event.relatedTarget
                    }
                });

        }
        else {
            interact(classCSS).draggable({
                enabled: false  // explicitly disable dragging
            });
        }
    });


};


