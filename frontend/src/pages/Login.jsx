import Cart from "@components/Carts/Cart";
import "../index.css";
import Link from "@components/Link";
import { Lock } from "lucide-react";


export  function Login(){
    

    return (
        <div className="  ">
            <div className="bg-primary-blue w-12 h-12  rounded-xl pt-2 pl-2 mx-auto mb-3">
               <Lock size={30} color="#ffffff" absoluteStrokeWidth  c/>
            </div>
            <div className="mb-6">
                <h1 className="text-3xl text-black font-bold" >Welcome Back</h1>
            <p className="mt-2 text-text-tertiary">Sign in to your inventory management account</p>

            </div>
            
        <Cart  />
        <footer className="mt-6">
             <div className="text-sm text-text-tertiary">By signing in, you agree to our 
                <Link to="" content=" Terms" /> of Service and 
          <Link to="" content=" Privacy Policy" /></div>
        </footer>
       
        </div>
    )
}