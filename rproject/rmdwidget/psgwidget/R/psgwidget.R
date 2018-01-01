#' <Add Title>
#'
#' <Add Description>
#'
#' @import htmlwidgets
#'
#' @export
psgwidget <- function(message, data, width = NULL, height = NULL, elementId = NULL) {

  # forward options using x
  arguments = list(
    message = message, data = data
  )

  # create widget
  htmlwidgets::createWidget(
    name = 'psgwidget',
    arguments,
    width = width,
    height = height,
    package = 'psgwidget',
    elementId = elementId
  )
}

#' Shiny bindings for psgwidget
#'
#' Output and render functions for using psgwidget within Shiny
#' applications and interactive Rmd documents.
#'
#' @param outputId output variable to read from
#' @param width,height Must be a valid CSS unit (like \code{'100\%'},
#'   \code{'400px'}, \code{'auto'}) or a number, which will be coerced to a
#'   string and have \code{'px'} appended.
#' @param expr An expression that generates a psgwidget
#' @param env The environment in which to evaluate \code{expr}.
#' @param quoted Is \code{expr} a quoted expression (with \code{quote()})? This
#'   is useful if you want to save an expression in a variable.
#'
#' @name psgwidget-shiny
#'
#' @export
psgwidgetOutput <- function(outputId, width = '100%', height = '400px'){
  htmlwidgets::shinyWidgetOutput(outputId, 'psgwidget', width, height, package = 'psgwidget')
}

#' @rdname psgwidget-shiny
#' @export
renderPsgwidget <- function(expr, env = parent.frame(), quoted = FALSE) {
  if (!quoted) { expr <- substitute(expr) } # force quoted
  htmlwidgets::shinyRenderWidget(expr, psgwidgetOutput, env, quoted = TRUE)
}
