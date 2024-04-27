import { Typography } from "@mui/material";
import { Popup, useMapEvents } from "react-leaflet";

export const LocationFinder: React.FC<{
  lat?: number;
  lng?: number;
  setLatLng: (latLng: { lat: number; lng: number }) => void;
  setCurrentZoom: (zoom: number) => void;
}> = ({ lat, lng, setLatLng, setCurrentZoom }) => {
  useMapEvents({
    click(e: any) {
      const { lat, lng } = e.latlng;
      setLatLng({ lat: lat.toFixed(6), lng: lng.toFixed(6) });
      setCurrentZoom(e.target._zoom);
    },
  });
  if (lat && lng) {
    return (
      <Popup position={[lat, lng]}>
        <Typography variant="body1" fontSize={15}>
          Lat: {lat}
          <br />
          Lng: {lng}
        </Typography>
      </Popup>
    );
  } else {
    return null;
  }
};
