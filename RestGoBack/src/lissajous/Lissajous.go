package lissajous

import (
	"image"
	"image/color"
	"image/gif"
	"io"
	"log"
	"math"
	"math/rand"
	"net/http"
)

var palette = []color.Color{
	color.White,
	color.RGBA{R: 255, G: 0, B: 0, A: 255},
	color.RGBA{R: 0, G: 255, B: 0, A: 255},
	color.RGBA{R: 0, G: 0, B: 255, A: 255},
	color.RGBA{R: 0, G: 255, B: 255, A: 255},
	color.RGBA{R: 255, G: 0, B: 255, A: 255},
	color.RGBA{R: 255, G: 255, B: 0, A: 255},
}

func lissajous(out io.Writer, cycles int) {
	const (
		nbColor = 6
		res    = 0.001
		size   = 300
		nframe = 64
		delay  = 8
	)
	freq := rand.Float64() * 3.0
	anim := gif.GIF{LoopCount: nframe}
	phase := 0.0
	for i := 0; i < nframe; i++ {
		rect := image.Rect(0, 0, 2*size+1, 2*size+1)
		img := image.NewPaletted(rect, palette)
		for t := 0.0; t < float64(cycles)*2*math.Pi; t += res {
			x := math.Sin(t)
			y := math.Sin(t*freq + phase)
			img.SetColorIndex(size+int(x*size+0.5), size+int(y*size+0.5), uint8((int(t)/nbColor)%6+1))
		}
		phase += 0.1
		anim.Delay = append(anim.Delay, delay)
		anim.Image = append(anim.Image, img)
	}
	_ = gif.EncodeAll(out, &anim)
}

func LissajousWeb(w http.ResponseWriter, r *http.Request) {
	lissajous(w, 50)
	log.Println("lissajou")
}

