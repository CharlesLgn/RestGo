package color

import (
	"fmt"
	"github.com/ant0ine/go-json-rest/rest"
	"math/rand"
	"strconv"
)

func RandomColor(w rest.ResponseWriter, r *rest.Request) {
	color := Color{}
	color.Blue = rand.Intn(256)
	color.Green = rand.Intn(256)
	color.Red = rand.Intn(256)

	var str = returnNb(color.Red) + returnNb(color.Green) + returnNb(color.Blue)
	color.Link = "https://www.google.com/search?q=%23" + str
	color.RGB = "#" + str
	color.Lum = getLum(color)
	color.Sat = getSat(color)
	w.WriteJson(color)
}

func returnNb(x int) string {
	res := strconv.FormatInt(int64(x), 16)
	if x < 16 {
		res = "0" + res
	}
	return res
}

func getLum(color Color) string {
	var x = getMax(color)
	var res = float64(x) / 255. * 100.
	return fmt.Sprintf("%.2f", res) + "%"
}

func getSat(color Color) string {
	var max = getMax(color)
	var min = getMin(color)
	var res = 100-(float64(min)/float64(max) * 100.)
	return fmt.Sprintf("%.2f", res) + "%"
}

func getMax(color Color) int {
	if color.Green > color.Blue && color.Green > color.Red {
		return color.Green
	} else if color.Blue > color.Red {
		return color.Blue
	} else {
		return color.Red
	}
}

func getMin(color Color) int {
	if color.Green < color.Blue && color.Green < color.Red {
		return color.Green
	} else if color.Blue < color.Red {
		return color.Blue
	} else {
		return color.Red
	}
}
