/*======================================
 * Chess Game Server
 * Jarno Tainio
 * jarno.tainio@tuni.fi
 =====================================*/
 
const express = require('express');
const helmet = require('helmet');
//const xss = requre('xss-filter');
//varbcrypt= require('bcryptjs’);
	/*
	app.post('/set', function(req, res){
		varpassword = req.body.newpassword;
		bcrypt.hash(password, saltRounds, function(err, hash){
			pass = hash;
			res.send('passwordset')
		});
	});
	bcrypt.compare(req.body.password, pass, function(err, result) {
		if(result) {
			req.session.authenticated=true;
			res.redirect('/');
		} else{
			res.status(401);
			res.end('loginfailed');
		}
	});
	*/

const mongoose = require('mongoose');
const bodyParser = require('body-parser')
const cors = require('cors')

const port = process.env.PORT || 3000;
const SUCCESS = "-success";

const app = express();
app.options('*', cors())
app.use(bodyParser.json())
app.use(helmet());

var uristring = process.env.MONGOLAB_URI || process.env.MONGOHQ_URL || 'mongodb://localhost/';
mongoose.connect(uristring, function (err, res) {
  if (err) { 
    console.log ('ERROR connecting to: ' + uristring + '. ' + err);
  } else {
    console.log ('Succeeded connected to: ' + uristring);
  }
});
	
var db = mongoose.connection;
var Schema = mongoose.Schema;

var CampaignSchema = new Schema({
	index: Number,									//Order of campaigns
	name: {type: String, default: "campaign"},		//Campaign name
	type: {type: String, default: "Open"},			//Open, Chain, Locked
	missionsOpen: {type: Number, default: 0},		//How many missions are automaticly open.
	image: {type: Number, default: 0},				//Image shown on mission selection screen
	icon: {type: Number, default: 0},				//Icon shown on mission selection screen
	map: [{type: Number}],							//Map tiles that are drawn to campaign info
});

var MissionSchema = new Schema({
	
	index: Number,									//Order of missions
	campaign: String,								//Owner campaigns _id

	name: {type: String, default: "mission"},		//Mission name
	description: {type: String, default: ""},		//Mission description shown on mission selection screen
	mapLocation: {type: Number, default: 0},		//Mission's location in campaign map
	width: {type: Number, default: 8},				//Board width
	height: {type: Number, default: 8},				//Board height
	tileSet: {type: Number, default: 0},			//Number of tileSet used for board
	
	aiDifficulty: {type: Number, default: 1},		
	aiRandom: {type: Number, default: 1},
	aiCanOnlyAttack: {type: Boolean, default: false},
	
	advisor: {type: Number, default: 0},
	message: [ {type : String} ],
	
	whiteArmy: [ {name : String, x: Number, y: Number} ],
	blackArmy: [ {name : String, x: Number, y: Number} ],
	neutralArmy:[{name : String, x: Number, y: Number} ],
	
	hiddenTurnLimit: {type: Boolean, default: false},
	turnLimit: {type: Number, default: -1},
	timeLimit: {type: Number, default: -1},
	targetX: {type: Number, default: -1},
	targetY: {type: Number, default: -1},
	objectives: [ {type : String} ],
	
	singleBlack: {type: Boolean, default: false},
	checkMate: {type: Boolean, default: true},
	blackCantMove: {type: Number, default: -2},
	whiteCantMove: {type: Number, default: -2},
	blackStarts: {type: Boolean, default: false},
	
	showTurns: {type: Boolean, default: true},
	showClock: {type: Boolean, default: false},
	showPlayerMoves: {type: Boolean, default: false},
	showErrorMoves: {type: Boolean, default: false},
	showEnemyMoves: {type: Boolean, default: false},
	
	autoLoadNext: {type: Boolean, default: true},
	
	showScore: {type: Boolean, default: true},
	showHighScore: {type: Boolean, default: false},
	silver: {type: Number, default: 90},
	gold: {type: Number, default: 100},
	victoryScore: {type: Number, default: 200},
	turnPenalty: {type: Number, default: 10},
	timePenalty: {type: Number, default: 0},
});

var Campaigns = mongoose.model('Campaign', CampaignSchema);
var Missions = mongoose.model('Mission', MissionSchema);

db.on('error', console.error.bind(console, 'MongoDB connection error:'));

app.get('/', async function (req, res) {
	res.end("Hello world!");
})

/*=======================================================================================
 * < C A M P A I G N S >
 ======================================================================================*/

//GET ALL CAMPAIGNS
//===============================================================
app.get('/campaigns', async function (req, res) {
	console.log("Campaign GET ALL");
	var campaignList = await Campaigns.find({});
	campaignList.forEach((campaign) => {
		campaign.link = campaign.name;
	})
	res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
	res.set('Content-Type', 'application/json');
	res.status(200);
	res.json(campaignList);
})

//GET SINGLE CAMPAIGNS
//===============================================================
app.get('/campaigns/:_id', function (req, res) {
	console.log("Campaign GET: " + req.params._id);
	res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
	if (req.params._id) {
		Campaigns.findOne({_id: req.params._id }, function(err, campaign) {
			if (err) {
				res.status(404);
				res.end("Error");
				return console.error(err);	
			}
			if (!campaign){
				res.status(404);
				res.end("Unknown id!");
			}
			if (campaign){
				campaign.link = campaign._id;
				res.set('Content-Type', 'application/json');
				res.status(200);
				res.json(campaign);
				console.log(SUCCESS);
			}
		});
	} else {
		res.status(400);
		res.send('Required information missing');
	}
})

//ADD CAMPAIGN
//===============================================================
app.post('/campaigns', function (req, res) {
	res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
	console.log("Campaign ADD: " + req.body.index);

    if (req.body.index != undefined) {
		var newCampaign = new Campaigns({
			index: req.body.index,
		});
		newCampaign.save(function(err) {
			if (err) return console.error(err);
			res.status(200);
			res.json(newCampaign);
			console.log(SUCCESS);
		});
	} else {
		res.status(400);
		res.send('Required information missing');
	}
})

//UPDATE CAMPAIGN
//===============================================================
app.patch('/campaigns/:_id', function (req, res) {
	res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
	console.log("Campaign UPDATE: " + req.body._id);

	if (req.params._id) {
		Campaigns.findOneAndUpdate({_id: req.params._id }, req.body, function(err, campaign) {
			if (!campaign){
				res.status(404);
				res.end("Unknown id!");
			}
			if (err) return handleError(err);
			
			if (campaign){
				res.status(200);
				res.json(campaign);
				console.log(SUCCESS);
			}
		});
	} else {
		res.status(400);
		res.send('Required information missing');
	}
})
	
//DELETE CAMPAIGN
//===============================================================
app.delete('/campaigns/:_id', async function (req, res) {
	res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
	console.log("Campaign DELETE: " + req.params._id);
	if (req.params._id) {
		try{
			console.log("try");
			const missions = await Missions.deleteMany( { campaign: req.params._id }, function(err, mission) {console.log(mission);});
			const campaign = await Campaigns.findOneAndDelete({_id: req.params._id }, function(err, campaign) {console.log(campaign);});
			res.status(200);
			res.send("Campaign deleted");
			console.log(SUCCESS);
		}
		catch(err){
			console.log(err);
			res.send('Failed to delete');
			res.status(400);
		}
	} else {
		res.status(400);
		res.send('Required information missing');
	}
})

/*=======================================================================================
 * < M I S S I O N S >
 ======================================================================================*/

//GET MISSIONS
//===============================================================
app.get('/missions/:_id', async function (req, res) {
	console.log("Missions GET: campaign: " + req.params._id);
	res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
		if (req.params._id) {
		try{
			const missions = await Missions.find( { campaign: req.params._id }, function(err, task) {})
			res.status(200);
			res.json(missions);
			console.log(SUCCESS);
		}
		catch(err){
			res.send(err);
			res.status(400);
		}
	} else {
		res.status(400);
		res.send('Required information missing');
	}
})

//GET ALL MISSIONS
//===============================================================
app.get('/missions', async function (req, res) {
	console.log("Missions GET ALL");
	res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
	try{
		const missions = await Missions.find({})
		res.status(200);
		res.json(missions);
		console.log(SUCCESS);
	}
	catch(err){
		res.send(err);
		res.status(400);
	}
})

//ADD MISSION
//===============================================================
app.post('/missions/:campaign', function (req, res) {
	res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With, content-type');
	console.log("Missions ADD: campaign: " + req.params.campaign);

    if (req.params.campaign) {
		var newMission = new Missions({
			//index: req.params.index,
			campaign: req.params.campaign,
		});
		newMission.save(function(err) {
			if (err) return console.error(err);
			res.status(200);
			res.json(newMission);
			console.log(SUCCESS);
		});
	} else {
		res.status(400);
		res.send('Required information missing');
	}
})

//DELETE MISSION
//===============================================================
app.delete('/missions/:_id', async function (req, res) {
	res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
	console.log("Mission DELETE: " + req.params._id);
	if (req.params._id) {
		try{
			const mission = await Missions.findOneAndDelete({_id: req.params._id }, function(err, task) {})
			res.status(200);
			res.send("Mission deleted");
			res.send();
			console.log(SUCCESS);
		}
		catch(err){
			res.status(400);
			res.send('Failed to delete');
		}
	} else {
		res.status(400);
		res.send('Required information missing');
	}
})

//UPDATE MISSION
//===============================================================
app.patch('/missions/:_id', function (req, res) {
	res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
	console.log("Mission UPDATE: " + req.body._id);
	console.log(req.body);
	if (req.params._id) {
		Missions.findOneAndUpdate({_id: req.params._id }, req.body, function(err, mission) {
			if (!mission){
				res.status(404);
				res.end("Unknown id!");
			}
			if (err) return handleError(err);
			
			if (mission){
				res.status(200);
				res.json(mission);
				console.log(SUCCESS);
			}
		});
	} else {
		res.status(400);
		res.send('Required information missing');
	}
})
	
app.listen(port, () => console.log('Listening on port ' + port))