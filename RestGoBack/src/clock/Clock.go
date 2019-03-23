package clock

import (
  "image"
  "image/color"
  "image/gif"
  "io"
  "log"
  "net/http"
  "time"
)

var palette = []color.Color{
  color.White,
  color.RGBA{R: 0, G: 0, B: 0, A: 255},
  color.RGBA{R: 100, G: 0, B: 0, A: 255},
}

func ClockWeb(w http.ResponseWriter, _ *http.Request) {
  h, min, sec := time.Now().Clock()
  clock(w, h, min, sec)
  log.Println("clock")
}

func clock(out io.Writer, h, min, sec int) {
  const (
    size   = 200
    delay  = 8
    nframe = 1
    color  = 1
    middle = 2
    rayon  = 200
  )
  h   = h%12
  min = min%60
  sec = sec%60

  anim := gif.GIF{LoopCount: nframe}
  rect := image.Rect(0, 0, 2*size+1, 2*size+1)
  img := image.NewPaletted(rect, palette)
  drawCircle(*img, size, size, rayon, color, 5)
  drawCircle(*img, size, size, 5, middle, 5)
  drawLine(*img, size, size, size+150, size+150, middle)
  anim.Delay = append(anim.Delay, delay)
  anim.Image = append(anim.Image, img)
  _ = gif.EncodeAll(out, &anim)
}

func drawCircle(img image.Paletted, x0, y0, r int, color int, border int) {
  for i := 0 ; i < border ; i++ {
    drawCircleOneCircle(img , x0, y0, r-i, color)
  }
}

func drawCircleOneCircle(img image.Paletted, x0, y0, r int, color int) {
  x, y, dx, dy := r-1, 0, 1, 1
  err := dx - (r * 2)
  for x >= y {
    img.SetColorIndex(x0+(x), y0+(y), uint8(color))
    img.SetColorIndex(x0+(y), y0+(x), uint8(color))
    img.SetColorIndex(x0-(y), y0+(x), uint8(color))
    img.SetColorIndex(x0-(x), y0+(y), uint8(color))
    img.SetColorIndex(x0-(x), y0-(y), uint8(color))
    img.SetColorIndex(x0-(y), y0-(x), uint8(color))
    img.SetColorIndex(x0+(y), y0-(x), uint8(color))
    img.SetColorIndex(x0+(x), y0-(y), uint8(color))

    if err <= 0 {
      y++
      err += dy
      dy += 2
    }
    if err > 0 {
      x--
      dx += 2
      err += dx - (r * 2)
    }
  }
}

func drawLine(img image.Paletted, x0, y0, x1, y1 int, color int)  {
  dx := x0 - x1
  dy := y0 - y1
  yi := 1
  if dy < 0 {
    yi = -1
    dy = -dy
  }
  D := 2*dy - dx
  y := y0
 /* if x0 > x1 {
    x0, x1 = reverse(x0, x1)
  }*/
  for x := x0; x < x1 ; x++ {
    img.SetColorIndex(x, y, uint8(color))
    if D > 0 {
      y = y + yi
      D = D - 2*dx
    }
    D = D + 2*dy
  }
}

func reverse(a,b int) (int, int){
  return b, a
}
