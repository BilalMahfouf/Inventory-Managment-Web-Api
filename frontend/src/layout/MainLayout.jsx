import { Outlet } from "react-router-dom";
import Layout from "./Layout";

export default function MainLayout() {
  return (
    <Layout>
      <Outlet /> {/* This is where child routes render */}
    </Layout>
  );
}